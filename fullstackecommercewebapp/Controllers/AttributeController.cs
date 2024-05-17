using AutoMapper;
using fullstackecommercewebapp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PagedList;

namespace fullstackecommercewebapp.Controllers
{
    [Authorize(Roles = "Administrator, Products Manager")]
    public class AttributeController : BaseController
    {
        public static string msg = "";
        public IActionResult Index(string sortOrder, string CurrentFilter, string SearchString, int? page)
        {
            ViewBag.msg = msg;
            msg = "";
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.CurrentSort = sortOrder;
            var attributes = _uow.attributeRepo.getAll();
            if (SearchString != null)
            {
                page = 1;
            }
            else
            {
                SearchString = CurrentFilter;
            }
            ViewBag.CurrentFilter = SearchString;
            if (!String.IsNullOrEmpty(SearchString))
            {
                attributes = attributes.Where(s => s.Name.ToLower().Contains(SearchString.ToLower()));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    attributes = attributes.OrderByDescending(s => s.Name);
                    break;
                default:
                    attributes = attributes.OrderBy(s => s.Name);
                    break;
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(attributes.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public IActionResult Add()
        {
            AttributeViewModel avm = new AttributeViewModel();
            return View(avm);
        }


        [HttpPost]
        public IActionResult Add(AttributeViewModel avm)
        {
            if (ModelState.IsValid)
            {
                var config = new MapperConfiguration(cfg => cfg.CreateMap<AttributeViewModel, Models.Attributes>());
                var mapper = new Mapper(config);
                fullstackecommercewebapp.Models.Attributes a = mapper.Map<Models.Attributes>(avm);
                if (_uow.attributeRepo.checkUnique(a.Name) != 0)
                {
                    ModelState.AddModelError("Name", "Attribute Name must be unique");
                    return View(avm);
                }
                _uow.attributeRepo.Add(a);
                _uow.SaveChanges();
            }
            else
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                return View(avm);
            }
            msg = "added";
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            Models.Attributes attribute = _uow.attributeRepo.getById(id);
            if (attribute == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.id = id;
            AttributeViewModel avm = new AttributeViewModel();
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Models.Attributes, AttributeViewModel>());
            var mapper = new Mapper(config);
            mapper.Map(attribute, avm);
            return View(avm);
        }


        [HttpPost]
        public IActionResult Edit(AttributeViewModel avm)
        {
            if (ModelState.IsValid)
            {
                Models.Attributes attribute = _uow.attributeRepo.getById(Convert.ToInt32(Request.Form["id"]));
                var config = new MapperConfiguration(cfg => cfg.CreateMap<AttributeViewModel, Models.Attributes>());
                var mapper = new Mapper(config);
                mapper.Map(avm, attribute);
                int check = _uow.attributeRepo.checkUnique(attribute.Name);
                System.Diagnostics.Debug.WriteLine(check);
                if (check != 0 && check != attribute.Id)
                {
                    ModelState.AddModelError("Name", "Attribute Name must be unique");
                    return View(avm);
                }
                _uow.attributeRepo.Edit(attribute);
                _uow.SaveChanges();
            }
            else
            {
                ViewBag.Id = Request.Form["id"];
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                return View(avm);
            }
            msg = "edited";
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var attribute = _uow.attributeRepo.getById(id);
            if (attribute == null)
            {
                return RedirectToAction("Index");
            }
            int pId = _uow.attributeRepo.checkDelete(id);
            if (pId == 0)
            {
                _uow.attributeRepo.Delete(id);
                _uow.SaveChanges();
                msg = "deleted";
                return RedirectToAction("Index");
            }
            msg = "NoDelete";
            return RedirectToAction("Index");

        }
    }
}

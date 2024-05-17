using AutoMapper;
using fullstackecommercewebapp.Data;
using fullstackecommercewebapp.Models;
using fullstackecommercewebapp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using PagedList;

namespace fullstackecommercewebapp.Controllers
{
    [Authorize(Roles = "Administrator, Products Manager")]
    public class CategoryController : BaseController
    {
        public static string msg = "";
        public IActionResult Index(string sortOrder, string CurrentFilter, string SearchString, int? page)
        {
            try
            {
                ViewBag.msg = msg;
                msg = "";
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                if (SearchString != null)
                {
                    page = 1;
                }
                else
                {
                    SearchString = CurrentFilter;
                }
                ViewBag.CurrentFilter = SearchString;
                var categories = _uow.categoryRepo.getAll();
                if (!String.IsNullOrEmpty(SearchString))
                {
                    categories = categories.Where(s => s.Name.ToLower().Contains(SearchString.ToLower()));
                }
                switch (sortOrder)
                {
                    case "name_desc":
                        categories = categories.OrderByDescending(s => s.Name);
                        break;
                    default:
                        categories = categories.OrderBy(s => s.Name);
                        break;
                }
                int pageSize = 5;
                int pageNumber = (page ?? 1);
                return View(categories.ToPagedList(pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                if (ex is SqlException)
                {
                    ViewBag.msg = "There is an Error Connecting to DataBase or Updating its Infromation. Try Later.";
                    return View("Error");
                }
                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult Add()
        {
            CategoryViewModel cvm = new CategoryViewModel();
            cvm.Attributes = _uow.attributeRepo.getAll();
            return View(cvm);
        }


        [HttpPost]
        public IActionResult Add(CategoryViewModel cvm)
        {

            if (ModelState.IsValid)
            {
                var config = new MapperConfiguration(cfg => cfg.CreateMap<CategoryViewModel, Category>());
                var mapper = new Mapper(config);
                Category p = mapper.Map<Category>(cvm);

                if (cvm.SelectedAttributes != null)
                {
                    foreach (var item in cvm.SelectedAttributes)
                    {
                        p.CategoryAttribute.Add(new CategoryAttribute() { Category = p, AttributeId = item });
                    }
                }
                if (_uow.categoryRepo.checkUnique(p.Name) != 0)
                {
                    ModelState.AddModelError("Name", "Category Name must be unique");
                    cvm.Attributes = _uow.attributeRepo.getAll();
                    return View(cvm);
                }
                if (cvm.AddedAttributes != null)
                {
                    foreach (var item in cvm.AddedAttributes)
                    {
                        if (item.Name == null || item.Name.Trim() == "")
                        {
                            ModelState.AddModelError("", "Added Attributes Names can not be empty");
                            cvm.Attributes = _uow.attributeRepo.getAll();
                            return View(cvm);
                        }
                        if (_uow.attributeRepo.checkUnique(item.Name) != 0)
                        {
                            _uow.attributeRepo.Add(item);
                        }
                        p.CategoryAttribute.Add(new CategoryAttribute() { Category = p, Attribute = item });
                    }
                }


                _uow.categoryRepo.Add(p);
                _uow.SaveChanges();
            }
            else
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                cvm.Attributes = _uow.attributeRepo.getAll();
                return View(cvm);
            }
            msg = "added";
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            Category category = _uow.categoryRepo.getById(id);
            if (category == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.id = id;
            CategoryViewModel p = new CategoryViewModel();
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Category, CategoryViewModel>());
            var mapper = new Mapper(config);
            mapper.Map(category, p);
            p.Attributes = _uow.attributeRepo.getAll();
            p.SelectedAttributes = _uow.categoryRepo.getCategoryAttributes(category.Id).Select(c => c.Id).ToList();

            return View(p);
        }


        [HttpPost]
        public IActionResult Edit(CategoryViewModel cvm)
        {
            if (ModelState.IsValid)
            {
                Category p = _uow.categoryRepo.getById(Convert.ToInt32(Request.Form["id"]));

                IEnumerable<Category> categories = _uow.categoryRepo.getAll();
                var config = new MapperConfiguration(cfg => cfg.CreateMap<CategoryViewModel, Category>());
                var mapper = new Mapper(config);
                mapper.Map(cvm, p);
                int check = _uow.categoryRepo.checkUnique(p.Name);
                System.Diagnostics.Debug.WriteLine(check);
                System.Diagnostics.Debug.WriteLine(Convert.ToInt32(Request.Form["id"]));
                if (check != 0 && check != p.Id)
                {
                    ModelState.AddModelError("Name", "Category Name must be unique");
                    cvm.Attributes = _uow.attributeRepo.getAll();
                    cvm.SelectedAttributes = _uow.categoryRepo.getCategoryAttributes(Convert.ToInt32(Request.Form["id"])).Select(c => c.Id).ToList();
                    return View(cvm);
                }

                _uow.categoryRepo.deleteCategoryAttributes(p.Id);

                if (cvm.SelectedAttributes != null)
                {
                    foreach (var item in cvm.SelectedAttributes)
                    {
                        p.CategoryAttribute.Add(new CategoryAttribute() { Category = p, AttributeId = item });
                    }
                }

                if (cvm.AddedAttributes != null)
                {
                    foreach (var item in cvm.AddedAttributes)
                    {
                        if (item.Name == null || item.Name.Trim() == "")
                        {
                            ModelState.AddModelError("", "Added Attributes Names can not be empty");
                            cvm.Attributes = _uow.attributeRepo.getAll();
                            return View(cvm);
                        }
                        if (_uow.attributeRepo.checkUnique(item.Name) != 0)
                            _uow.attributeRepo.Add(item);
                        p.CategoryAttribute.Add(new CategoryAttribute() { Category = p, Attribute = item });
                    }
                }



                _uow.categoryRepo.Edit(p);
                _uow.SaveChanges();
            }
            else
            {
                ViewBag.Id = Request.Form["id"];
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                cvm.Attributes = _uow.attributeRepo.getAll();
                cvm.SelectedAttributes = _uow.categoryRepo.getCategoryAttributes(Convert.ToInt32(Request.Form["id"])).Select(c => c.Id).ToList();
                return View(cvm);
            }
            msg = "edited";
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var category = _uow.categoryRepo.getById(id);
            if (category == null)
            {
                return RedirectToAction("Index");
            }
            int check = _uow.productRepo.getAll().Where(p => p.CategoryId == id).Count();
            if (check != 0)
            {
                msg = "NoDelete";
            }
            else
            {
                _uow.categoryRepo.Delete(id);
                _uow.SaveChanges();
                msg = "deleted";
            }
            return RedirectToAction("Index");

        }

        public IActionResult Details(int id)
        {
            var s = _uow.categoryRepo.getWithAttributeById(id);
            if (s == null)
            {
                return RedirectToAction("Index");
            }
            CategoryViewModel p = new CategoryViewModel();
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Category, CategoryViewModel>());
            var mapper = new Mapper(config);
            mapper.Map(s, p);
            List<Models.Attributes> attrs = new List<Models.Attributes>();

            if (s.CategoryAttribute != null)
            {
                foreach (var item in s.CategoryAttribute)
                {
                    attrs.Add(_uow.attributeRepo.getById(item.AttributeId));
                }
                p.CategoryAttributes = attrs;
            }
            ViewBag.id = id;
            ViewBag.CreatedAt = s.CreatedAt;
            ViewBag.ModifiedAt = s.UpdatedAt;
            p.cat_products = _uow.categoryRepo.getCategoryProducts(id).Take(3);
            return View(p);
        }
    }
}

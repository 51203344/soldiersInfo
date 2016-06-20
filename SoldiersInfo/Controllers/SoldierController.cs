using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SoldiersInfo.Models;
using PagedList;
using System.IO;
using iTextSharp.text;

namespace SoldiersInfo.Controllers
{
    public class SoldierController : Controller
    {
        private SoldiersInfoContext db = new SoldiersInfoContext();

        private int page_size = 6; // mỗi trang chỉ hiện 6 chiến sĩ
        public IEnumerable<SelectListItem> searchOptionList = new[] // những giá trị có thể dùng để tìm kiếm
        {
            new SelectListItem { Text = "Họ tên", Value = "name" },
            new SelectListItem { Text = "Ngày vào CAND", Value = "servingDate" },
            new SelectListItem { Text = "Thời điểm hưởng khởi điểm", Value = "pointDate" },
            new SelectListItem { Text = "Đơn vị", Value = "company" },
            new SelectListItem { Text = "Không rõ", Value = "unknow" }
        };
        //public String[] decider = { "Phạm Văn Vĩnh", "Thượng tá", "PHÓ TRƯỞNG PHÒNG" };
        //public String[] receiver = { "Như trên", "Lưu: PX13(CSBH)" };

        // GET: Soldier
        public ActionResult Index(string sortOrder, string currentFilter, string searchOption, string searchString, int? page, string message)
        {
            int fieldSearchSelected = fieldSearch(searchOption); // searchOption vô hiệu (invalid) được xem như "unknow"

            var soldiers = from s in db.Soldiers.Where(s => s.isDisplay == true) // chỉ lấy những chiến sĩ không bị xóa
                           select s;

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            // TÌM KIẾM
            if (!String.IsNullOrEmpty(searchString)) // nếu searchString không rỗng
                if (!String.IsNullOrEmpty(searchOption)) // nếu searchOption không rỗng (đương nhiên phù hợp để tìm kiếm)
                    soldiers = Searching.Search(soldiers, searchString, searchOption);
                else // nếu searchOption rỗng => tìm kiếm với giá trị "unknow"
                    soldiers = Searching.Search(soldiers, searchString, "unknow");

            //else // searchString thì bỏ qua searchOption

            // SẮP XẾP
            soldiers = sort(soldiers, sortOrder); // khi sortOrder vô hiệu (invalid),sortOrder tính theo tên

            ViewBag.CurrentFilter = searchString; // từ đang tìm
            ViewBag.SearchOption = searchOption; // nơi để tìm
            ViewBag.CurrentSort = sortOrder;  // chuyển tới view thứ tự sắp xếp
            // lấy thứ tự sắp xếp hiện tại, chuyển tới view
            ViewBag.LastNameSortParm = Sort_Order(sortOrder, 0);    // 0
            ViewBag.FirstNameSortParm = Sort_Order(sortOrder, 1);   // 1
            ViewBag.BirthdaySortParm = Sort_Order(sortOrder, 2);    // 2
            ViewBag.CompSortParm = Sort_Order(sortOrder, 3);        // 3
            ViewBag.ServingDateSortParm = Sort_Order(sortOrder, 4); // 4
            ViewBag.PointDateSortParm = Sort_Order(sortOrder, 5);   // 5
            ViewBag.AnnouceSortParm = Sort_Order(sortOrder, 6);     // 6
            searchOptionList.ElementAt(fieldSearchSelected).Selected = true; // đặt searchOption hiện tại thành true
            ViewBag.SearchOptionList = searchOptionList; // để chuyển tới view
            ViewBag.Message = message;

            ViewBag.Modelsize = soldiers.Count(); // số lượng chiến sĩ thỏa yêu cầu
            int pageSize = page_size;
            if (!String.IsNullOrEmpty(searchString) && !String.IsNullOrEmpty(searchOption)) // số trang hiển thị kết quả

                pageSize = (page_size < soldiers.Count()) ? soldiers.Count(): page_size;

            int pageNumber = (page ?? 1);
            return View(soldiers.ToPagedList(pageNumber, pageSize));
        }
        private IQueryable<Soldier> sort(IQueryable<Soldier> soldiers, string sortOrder)
        {
            switch (sortOrder)
            {
                default:
                case "":
                case "firstname_asc":
                    soldiers = soldiers.OrderBy(s => s.firstName);
                    break;
                case "firstname_desc":
                    soldiers = soldiers.OrderByDescending(s => s.firstName);
                    break;
                case "lastname_asc":
                    soldiers = soldiers.OrderBy(s => s.lastName);
                    break;
                case "lastname_desc":
                    soldiers = soldiers.OrderByDescending(s => s.lastName);
                    break;
                case "birthday_asc":
                    soldiers = soldiers.OrderBy(s => s.birthday);
                    break;
                case "birthday_desc":
                    soldiers = soldiers.OrderByDescending(s => s.birthday);
                    break;
                case "company_asc":
                    soldiers = soldiers.OrderBy(s => s.company);
                    break;
                case "company_desc":
                    soldiers = soldiers.OrderByDescending(s => s.company);
                    break;
                case "servingDate_asc":
                    soldiers = soldiers.OrderBy(s => s.servingDate);
                    break;
                case "servingDate_desc":
                    soldiers = soldiers.OrderByDescending(s => s.servingDate);
                    break;
                case "pointDate_asc":
                    soldiers = soldiers.OrderBy(s => s.pointDate);
                    break;
                case "pointDate_desc":
                    soldiers = soldiers.OrderByDescending(s => s.pointDate);
                    break;
                case "annouce_asc":
                    soldiers = soldiers.OrderBy(s => s.annouce);
                    break;
                case "annouce_desc":
                    soldiers = soldiers.OrderByDescending(s => s.annouce);
                    break;
            }
            return soldiers;
        }
        private String Sort_Order(string sortOrder, int type)
        {
            switch (type)
            {
                case 0: // lastname
                    sortOrder = sortOrder == "lastname_asc" ? "lastname_desc" : "lastname_asc";
                    break;
                default:
                case 1: // firstname
                    sortOrder = String.IsNullOrEmpty(sortOrder) ? "firstname_desc" : ""; // no need of firstname_asc and allow invalid sortOrder
                    break;
                case 2: // birthday
                    sortOrder = sortOrder == "birthday_asc" ? "birthday_desc" : "birthday_asc";
                    break;
                case 3: // company
                    sortOrder = sortOrder == "company_asc" ? "company_desc" : "company_asc";
                    break;
                case 4: // servingdate
                    sortOrder = sortOrder == "servingDate_asc" ? "servingDate_desc" : "servingDate_asc";
                    break;
                case 5: // pointdate
                    sortOrder = sortOrder == "pointDate_asc" ? "pointDate_desc" : "pointDate_asc";
                    break;
                case 6: // annouce
                    sortOrder = sortOrder == "annouce_asc" ? "annouce_desc" : "annouce_asc";
                    break;
            }
            return sortOrder;
        }
        private int fieldSearch(string searchOption)
        {
            int i = 4;
            switch (searchOption)
            {
                default:
                    break;
                case "name":
                    i = 0;
                    break;
                case "servingDate":
                    i = 1;
                    break;
                case "pointDate":
                    i = 2;
                    break;
                case "company":
                    i = 3;
                    break;
                case "unknow":
                    i = 4;
                    break;
            }
            return i;
        }
        // GET: Soldier/Details/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Print(String soldier_ID, String company, string decisionString, DateTime decisionDate, String deciderName, String deciderArmyRank, String deciderRight, String receiver)
        {
            //declare document
            float documentMarginLeft = Utilities.MillimetersToPoints(30);
            float documentMarginRight = Utilities.MillimetersToPoints(15);
            float documentMarginTopBottom = Utilities.MillimetersToPoints(20);
            // Document document = new Document(PageSize.A4, documentMarginLeft, documentMarginRight, documentMarginTopBottom, documentMarginTopBottom);
            MemoryStream stream = new MemoryStream();

            // fix soldier_ID from string to array of int
            soldier_ID = soldier_ID.Substring(0, soldier_ID.Length - 1); // remove last ";"
            String[] soldier_ID_String_Arrray = soldier_ID.Split(';'); // transform to array of string
            int[] soldier_ID_Int_Array = new int[soldier_ID_String_Arrray.Length];
            bool couldConvertToInt = true;
            int id_temp = 0;
            for (int i = 0; i < soldier_ID_String_Arrray.Length; i++)
            {
                couldConvertToInt = int.TryParse(soldier_ID_String_Arrray[i], out id_temp); // convert string to int
                if (couldConvertToInt)
                    soldier_ID_Int_Array[i] = id_temp;
                else
                    soldier_ID_Int_Array[i] = 0; // if cannot convert string to int, current id is 0
            }

            String[,] list_soldiers = stringArray_List_Soldiers(soldier_ID_Int_Array); // take list of soldiers
            String[] signer = { deciderRight, deciderArmyRank, deciderName }; // create list of sign elements

            // fix receiver 
            receiver = receiver.Substring(0, receiver.Length - 1); // remove last ";"
            String[] receiverArray = receiver.Split(';');
            stream = Printing.streamPrint(decisionString, decisionDate, list_soldiers, signer, receiverArray, company);
            String filename = "Report " + DateTime.Now.ToString() + ".pdf";
            return File(stream, "application/pdf", filename);
        }
        private String[,] stringArray_List_Soldiers(int[] source)
        {
            /*
             *  This function's purpose is to take list of soldiers to be printing
                We will take 5 properties of Soldier model: Name, birthday, serveDate, pointDate, note in order.
            */
            IQueryable<Soldier> allSoldiers = from s in db.Soldiers.Where(s => s.isDisplay == true)// only not-deleted soldier can be printed
                                              select s; ;
            List<Soldier> soldiers = new List<Soldier>();
            int current_id;
            IQueryable<Soldier> current_Soldier;
            // convert form IEnumerable to List
            if (source.Length < 1)
                soldiers = allSoldiers.ToList();
            else
                for (int i = 0; i < source.Length; i++) // more than 1 soldier
                {
                    current_id = source[i];
                    current_Soldier = allSoldiers.Where(s => s.ID == current_id);
                    soldiers.Add(current_Soldier.FirstOrDefault());
                }

            int list_length = source.Length;
            String[,] result = new String[soldiers.Count(), 5]; // take 5 properties

            String name;
            for (int h = 0; h < list_length; h++)
            {
                name = soldiers[h].lastName + " ";
                if (!String.IsNullOrEmpty(soldiers[h].middleName))
                    name += soldiers[h].middleName + " ";
                name += soldiers[h].firstName;
                result[h, 0] = name;
                result[h, 1] = soldiers[h].birthday.Year.ToString();
                result[h, 2] = soldiers[h].servingDate.Date.ToString("d"); // take Date only
                result[h, 3] = soldiers[h].pointDate.Date.ToString("d");
                if (!String.IsNullOrEmpty(soldiers[h].note))
                    result[h, 4] = soldiers[h].note.ToString();
                else
                    result[h, 4] = "";
            }
            return result;
        }
        public ActionResult Error()
        {
            return View();
        }
        public ActionResult ImportData()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ImportDataFromFile()
        {
            string mess = "Vui lòng nhập tập tin Excel";
            string[] allowedFileExtensions = new string[] { ".xlsx" };
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                if (file.ContentLength > 0)
                {
                    string fileExtension = file.FileName.Substring(file.FileName.LastIndexOf('.'));
                    if (allowedFileExtensions.Contains(fileExtension))
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/Upload"), fileName);
                        file.SaveAs(path);
                        mess = "Nhận tập tin thành công ";
                        int countOnSoldier = db.Soldiers.Where(s => s.firstName != null).Count();
                        mess += Import.ImportData(path, countOnSoldier);
                        System.IO.File.Delete(path);
                    }
                }
                else RedirectToAction("Error", new { message = mess });
            }
            else RedirectToAction("Error", new { message = mess });
            ViewBag.Message = mess;
            return RedirectToAction("Index", new { message = mess });
        }
        // GET: Soldier/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Soldier soldier = db.Soldiers.Find(id);
            if (soldier == null || soldier.isDisplay == false)
            {
                return HttpNotFound();
            }
            return View(soldier);
        }
        // GET: Soldier/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Soldier/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,lastName,middleName,firstName,birthday,company,servingDate,pointDate,note,annouce")] Soldier soldier)
        {
            Soldier soldier_toAdd = new Soldier();
            soldier_toAdd.ID = soldier.ID;
            soldier_toAdd.lastName = soldier.lastName;
            soldier_toAdd.middleName = soldier.middleName;
            soldier_toAdd.firstName = soldier.firstName;
            soldier_toAdd.birthday = soldier.birthday;
            soldier_toAdd.company = soldier.company;
            soldier_toAdd.servingDate = soldier.servingDate;
            soldier_toAdd.pointDate = soldier.pointDate;
            soldier_toAdd.note = soldier.note;
            soldier_toAdd.annouce = soldier.annouce;
            soldier_toAdd.isDisplay = true;
            if (ModelState.IsValid)
            {
                db.Soldiers.Add(soldier_toAdd);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
                //{
                //    ModelState.AddModelError("birthday", "Vui lòng điền ngày đúng định dạng dd/MM/yyyy!");
                //    ModelState.AddModelError("servingDate", "Vui lòng điền ngày đúng định dạng dd/MM/yyyy!");
                //    ModelState.AddModelError("pointDate", "error!");
                //}

                return View(soldier);
        }

        // GET: Soldier/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Soldier soldier = db.Soldiers.Find(id);
            if (soldier == null || soldier.isDisplay == false)
            {
                return HttpNotFound();
            }
            return View(soldier);
        }

        // POST: Soldier/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Soldier soldier)
        {
            var soldierToUpdate = db.Soldiers.Find(soldier.ID);
            if (soldierToUpdate == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                soldierToUpdate.isDisplay = true;
                soldierToUpdate.firstName = soldier.firstName;
                soldierToUpdate.middleName = soldier.middleName;
                soldierToUpdate.lastName = soldier.lastName;
                soldierToUpdate.birthday = soldier.birthday;
                soldierToUpdate.pointDate = soldier.pointDate;
                soldierToUpdate.note = soldier.note;
                soldierToUpdate.servingDate = soldier.servingDate;
                soldierToUpdate.annouce = soldier.annouce;
                soldierToUpdate.company = soldier.company.ToUpper();
                //db.Entry(soldier).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(soldierToUpdate);
        }

        // GET: Soldier/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Soldier soldier = db.Soldiers.Find(id);
            if (soldier == null || soldier.isDisplay == false)
            {
                return HttpNotFound();
            }
            return View(soldier);
        }

        // POST: Soldier/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Soldier soldier = db.Soldiers.Find(id);
            //db.Soldiers.Remove(soldier);
            db.Entry(soldier).State = EntityState.Modified;
            soldier.isDisplay = false; // không xóa, chỉ sửa trạng thái
            soldier.note = "Đã xóa";
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //public 
    }
}

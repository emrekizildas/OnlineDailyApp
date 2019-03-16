using Microsoft.AspNet.Identity;
using OnlineGunluk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OnlineGunluk.Controllers
{
    [Authorize]
    public class MainController : ApiController
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public IEnumerable<NotBaslikDTO> GetBasliklar()
        {
            string loggedInUserID = User.Identity.GetUserId();
            return db.Notlar.Where(x => x.ApplicationUserID == loggedInUserID).Select(x => new NotBaslikDTO { ID = x.ID, Baslik = x.Baslik }).ToList();
        }

        public IHttpActionResult GetNot(int id)
        {
            string loggedInUserID = User.Identity.GetUserId();
            Not not = db.Notlar.FirstOrDefault(x => x.ApplicationUserID == loggedInUserID && x.ID == id);
            if (not == null)
                return NotFound();

            return Ok(not);
        }

        [HttpPost]
        public HttpResponseMessage Save(NotKaydetDTO not)
        {
            string loggedInUserID = User.Identity.GetUserId();
            NotIslemBilgiDTO mesaj = new NotIslemBilgiDTO();
            if (not.ID == 0)
            {
                //Yeni kayıt eklenecek...
                Not n = new Not();
                n.ApplicationUserID = loggedInUserID;
                n.Baslik = not.Baslik;
                n.EklenmeTarihi = DateTime.Now;
                n.Icerik = not.Icerik;
                db.Notlar.Add(n);
                db.SaveChanges();
                mesaj.ID = n.ID;
                mesaj.Mesaj = "Eklendi (" + n.EklenmeTarihi.Value.ToShortDateString() + ")";
            }
            else
            {
                //Güncelleme yapılacak...
                Not n = db.Notlar.FirstOrDefault(x => x.ApplicationUserID == loggedInUserID && x.ID == not.ID);
                if (n == null)
                    return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Bir hata oluştu...");

                n.Baslik = not.Baslik;
                n.Icerik = not.Icerik;
                n.SonGuncellenmeTarihi = DateTime.Now;
                db.SaveChanges();
                mesaj.ID = n.ID;
                mesaj.Mesaj = "Güncellendi (" + n.SonGuncellenmeTarihi.Value.ToShortDateString() + ")";
            }

            

            return Request.CreateResponse(HttpStatusCode.OK, mesaj);
        }

        [HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            string loggedInUserID = User.Identity.GetUserId();
            Not not = db.Notlar.FirstOrDefault(x => x.ApplicationUserID == loggedInUserID  && x.ID == id);
            if (not == null)
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Bulunamadı");

            string baslik = not.Baslik;

            db.Notlar.Remove(not);
            db.SaveChanges();

            NotIslemBilgiDTO mesaj = new NotIslemBilgiDTO()
            {
                ID = id,
                Mesaj = baslik + " başlıklı not silindi..."
            };

            return Request.CreateResponse(HttpStatusCode.OK, mesaj);
        }
    }
}

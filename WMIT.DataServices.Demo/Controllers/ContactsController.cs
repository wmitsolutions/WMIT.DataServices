using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.OData;
using WMIT.DataServices.Common;
using WMIT.DataServices.Common.Attributes;
using WMIT.DataServices.Controllers;
using WMIT.DataServices.Demo.Models;

namespace WMIT.DataServices.Demo.Controllers
{
    public class ContactsController : ODataController<AddressBookDB, Contact>
    {
        [ODataAction]
        [ODataProcedureParameter(Name="test", Type=typeof(int))]
        public int AddTag(ODataActionParameters parameters)
        {
            return 5;
        }

        [ODataAction(ReturnsEntity=typeof(IEnumerable<Contact>))]
        [ODataProcedureParameter(Name = "tagIds", Type = typeof(IEnumerable<int>))]
        public IHttpActionResult SetTags([FromODataUri]int key, ODataActionParameters parameters)
        {
            var tagIds = (IEnumerable<int>)parameters["tagIds"];

            //var family = db.Families
            //    .Single(x => x.Id == key);

            //var tags = db.Tags
            //    .Where(x => tagIds.Contains(x.Id));

            //family.Tags.Clear();

            //foreach (var tag in tags)
            //{
            //    family.Tags.Add(tag);
            //}
            //db.SaveChanges();
            var contact = Entities.SingleOrDefault(e => e.Id == 1);
            return Ok(tagIds.ToList());
        }
    }
}
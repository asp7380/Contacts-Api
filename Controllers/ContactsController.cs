using Contacts_Api.Models;
using LiteDB;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Contacts_Api.Controllers
{
    public class ContactsController : ApiController
    {
        // GET: api/Contacts
        public IList<Contact> GetContacts()
        {
            using (var db = new LiteDatabase(ConfigurationManager.AppSettings["DefaultConn"]))
            {
                IList<Contact> contacts;
                IList<Address> address;
                IList<Phone> phone;

                var contactList = db.GetCollection<Contact>("contacts");
                var addrList = db.GetCollection<Address>("address");
                var phoneList = db.GetCollection<Phone>("phone");

                contacts = contactList.FindAll().ToList();
                address = addrList.FindAll().ToList();
                phone = phoneList.FindAll().ToList();
                

                foreach(var contact in contacts)
                {
                    contact.address = address.Where(a => a.fId == contact.Id).ToList();
                    contact.phone = phone.Where(p => p.fId == contact.Id).ToList();
                }

                return contacts;

            }

        }


        // GET: api/Contacts/2
        public IList<Contact> Get(int id)
        {
            using (var db = new LiteDatabase(ConfigurationManager.AppSettings["DefaultConn"]))
            {
                IList<Contact> contacts;
                IList<Address> address;
                IList<Phone> phone;

                var contactList = db.GetCollection<Contact>("contacts");
                var addrList = db.GetCollection<Address>("address");
                var phoneList = db.GetCollection<Phone>("phone");

                contacts = contactList.FindAll().Where(c => c.Id == id).ToList();
                address = addrList.FindAll().Where(a => a.fId == id).ToList();
                phone = phoneList.FindAll().Where(p => p.fId == id).ToList();


                foreach (var contact in contacts)
                {
                    contact.address = address.Where(a => a.fId == contact.Id).ToList();
                    contact.phone = phone.Where(p => p.fId == contact.Id).ToList();
                }

                return contacts;
            }

        }



        // POST: api/Contacts
        public void PostContact([FromBody]Contact contact)
        {
            using (var db = new LiteDatabase(ConfigurationManager.AppSettings["DefaultConn"]))
            {
                var contactCollection = db.GetCollection<Contact>("contacts");
                var addrCollection = db.GetCollection<Address>("address");
                var phoneCollection = db.GetCollection<Phone>("phone");

                int contactListId = 0;
                
                //Build new contact, Then address right after to create relationship
                var newcontactList = new Contact
                {
                    First = contact.First,
                    Middle = contact.Middle,
                    Last = contact.Last,
                    Email = contact.Email
                };

                //Append new contact to DB.
                contactCollection.Insert(newcontactList);

                //Retrieve Contacts
                var contactList = contactCollection.FindAll();

                //Should have at least one contact now, if not, we need to find out why and fix before going any further.
                if (contactList != null)
                {
                    //Build a primary key / foriegn key relationship.
                    //contactListId = contactList.OrderByDescending(o => o.Id).Select(c => c.Id).FirstOrDefault() == 1 ?
                    //   contactList.OrderByDescending(o => o.Id).Select(c => c.Id).FirstOrDefault() 
                    //   : (contactList.OrderByDescending(o => o.Id).Select(c => c.Id).FirstOrDefault() + 1);

                    contactListId = contactList.OrderByDescending(o => o.Id).Select(c => c.Id).FirstOrDefault();


                    //Loop through any addresses stored on the contact, and append to the database
                    foreach (var addressInfo in contact.address)
                    {

                        var newaddrList = new Address
                        {
                            fId = contactListId, //Build Relationship
                            Street = addressInfo.Street,
                            City = addressInfo.City,
                            Zip = addressInfo.Zip,
                            State = addressInfo.State

                        };

                        addrCollection.Insert(newaddrList);

                    }

                    //Loop through any phone numbers stored on the contact, and append to the database
                    foreach (var phoneInfo in contact.phone)
                    {
                        var newPhoneList = new Phone
                        {
                            fId = contactListId, //Build Relationship
                            phoneNbr = phoneInfo.phoneNbr,
                            phoneType = phoneInfo.phoneType
                        };

                        phoneCollection.Insert(newPhoneList);

                    }

                }

            }
        }

        // PUT: api/Contacts/5
        public void Put(int id, [FromBody]Contact contact)
        {
            using (var db = new LiteDatabase(ConfigurationManager.AppSettings["DefaultConn"]))
            {
                var contactsCollection = db.GetCollection<Contact>("contacts");
                var addressCollection = db.GetCollection<Address>("address");
                var phoneCollection = db.GetCollection<Phone>("phone");

                var contactsEntity = contactsCollection.FindById(id);

                //Update Contact
                contactsEntity.First = contact.First;
                contactsEntity.Middle = contact.Middle;
                contactsEntity.Last = contact.Last;
                contactsEntity.Email = contact.Email;

                contactsCollection.Update(contactsEntity);


                var addressEntity = addressCollection.FindAll().Where(a => a.fId == id).OrderBy(a => a.Id).ToList();
                var phoneEntity = phoneCollection.FindAll().Where(p => p.fId == id).OrderBy(a => a.Id).ToList();


                    //Update Address Info
                    if (addressEntity != null)
                    {
                        var cntr = 1;
                        foreach (var addrItem in addressEntity)
                        {
                            addrItem.Street = contact.address[cntr - 1].Street;
                            addrItem.City = contact.address[cntr - 1].City;
                            addrItem.State = contact.address[cntr - 1].State;
                            addrItem.Zip = contact.address[cntr - 1].Zip;

                            addressCollection.Update(addressEntity);
                            cntr += 1;
                        }
                    }

                    //Update Phone Info
                    if (phoneEntity != null)
                    {
                        var cntr = 1;
                        foreach (var phoneItem in phoneEntity)
                        {
                            phoneItem.phoneNbr = contact.phone[cntr - 1].phoneNbr;
                            phoneItem.phoneType = contact.phone[cntr - 1].phoneType;

                            phoneCollection.Update(phoneEntity);
                            cntr += 1;

                        }

                    }
            }
        }


        // DELETE: api/Contacts/5
        public void Delete(int id)
        {
            using (var db = new LiteDatabase(ConfigurationManager.AppSettings["DefaultConn"]))
            {

                IList<Address> address;
                IList<Phone> phone;

                //Remove Contact
                var contactList = db.GetCollection<Contact>("contacts");

                if(contactList != null)
                contactList.Delete(id);

                //Remove Address
                var addrList = db.GetCollection<Address>("address");
                address = addrList.FindAll().Where(a => a.fId == id).ToList();

                if (addrList != null)
                {
                    foreach(var addrItem in address)
                    {
                        addrList.Delete(addrItem.Id);
                    }

                }
                

                //Remove Phone
                var phoneList = db.GetCollection<Phone>("phone");
                phone = phoneList.FindAll().Where(p => p.fId == id).ToList();

                if (phoneList != null)
                {
                    foreach(var phoneItem in phone)
                    {
                        phoneList.Delete(phoneItem.Id);
                    }

                }
                

            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Server.Entities
{
    // test pr
    public class Category
    {
        private int _id;
        private string _name;

        public Category(int id, string name)
        {
            _id = id;
            _name = name;
        }
        [Key]
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }
}
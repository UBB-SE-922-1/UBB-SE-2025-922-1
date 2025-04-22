using System;
using System.Collections.Generic;

namespace Duo.Models
{
    public class Hashtag
    {
        private int _id;
        private string _tag;

        public Hashtag(int id, string tag)
        {
            _id = id;
            _tag = tag;
        }

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }

        public string Name
        {
            get { return _tag; }
            set { _tag = value; }
        }
    }
}
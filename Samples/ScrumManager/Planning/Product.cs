using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace dotObjects.Samples.ScrumManager.Planning
{
    public class Product
    {
        private string name;
        private string description;
        private Member master;
        private List<BacklogItem> backlog;
        private List<Member> members;
        private List<Iteration> sprints;

        public Product(string name, string description, Member master)
        {
            Name = name;
            Description = description;
            Master = master;
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public Member Master
        {
            get { return master; }
            set { master = value; }
        }

        internal List<BacklogItem> Backlog
        {
             get
            {
                if (backlog == null)
                    backlog = new List<BacklogItem>();
                return backlog;
            }
            set { backlog = value; }
        }

        internal List<Iteration> Sprints
        {
            get
            {
                if (sprints == null)
                    sprints = new List<Iteration>();
                return sprints;
            }
            set { sprints = value; }
        }

        public List<Member> Members
        {
            get
            {
                if (members == null)
                    members = new List<Member>();
                return members;
            }
            set { members = value; }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
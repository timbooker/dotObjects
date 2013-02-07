using System;
using System.Collections.Generic;

namespace dotObjects.Samples.ScrumManager.Planning
{
    public class Release
    {
        private string name;
        private DateTime date;
        private List<Iteration> iterations;

        public Release(string name, DateTime date)
        {
            Name = name;
            Date = date;
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }

        public List<Iteration> Iterations
        {
            get
            {
                if (iterations == null)
                    iterations = new List<Iteration>();
                return iterations;
            }
            set { iterations = value; }
        }

        public override string ToString()
        {
            return Date.ToShortDateString() + " - " + Name;
        }
    }
}

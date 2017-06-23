using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PlanningPoker.Models;

namespace PlanningPoker.Services
{
    public class ScaleService
    {
        private List<Scale> marks;

        public ScaleService()
        {
            if (marks == null)
                marks = new List<Scale>
                {
                    new Scale {mark = "0"},
                    new Scale {mark = "1/2"},
                    new Scale {mark = "1"},
                    new Scale {mark = "2"},
                    new Scale {mark = "3"},
                    new Scale {mark = "5"},
                    new Scale {mark = "8"},
                    new Scale {mark = "13"},
                    new Scale {mark = "20"},
                    new Scale {mark = "40"},
                    new Scale {mark = "100"},
                    new Scale {mark = "∞"},
                    new Scale {mark = "?"},
                    new Scale {mark = "koffee"}
                };
        }

        public List<Scale> GetMarks()
        {
            return marks;
        }
    }
}
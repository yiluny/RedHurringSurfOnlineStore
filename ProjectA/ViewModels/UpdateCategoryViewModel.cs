using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectA.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectA.ViewModels
{
    public class UpdateCategoryViewModel
    {
        public List<SubCategory> existingData { get; set; }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TabloidCLI.Models
{
    public class Note
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime PublishDateTime { get; set; }
        public Post Post { get; set; }

       
    }
}


﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MKV_Chapterizer.bin
{
    public partial class ChapterDB : Form
    {
        public ChapterDB()
        {
            InitializeComponent();
        }

        private void ChapterDB_Load(object sender, EventArgs e)
        {
            
        }

        public void SearchChapters(string movieName)
        {
            txtboxSearchName.Text = movieName;
            
        }
    }
}

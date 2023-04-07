using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GFin.Web.Models
{
    public class ChartModel
    {
        public ChartModel()
        {
            data = new Data();
            options = new Options();
        }
        public string type { get; set; }
        public Data data { get; set; }
        public Options options { get; set; }
    }

    public class Data
    {
        public string[] labels { get; set; }
        public List<Datasets> datasets { get; set; }
    }
    public class Datasets
    {
        public Datasets()
        {
            backgroundColor = new string[0];
            borderColor = new string[0];
            data = new string[0];
            label = string.Empty;
            borderWidth = 0;
        }
        public string label { get; set; }
        public string[] backgroundColor { get; set; }
        public string[] borderColor { get; set; }
        public int borderWidth { get; set; }
        public string[] data { get; set; }
        public bool fill { get; set; }
    }
    public class Options
    {
        public Options()
        {
            title = new Title();
            scales = new Scales();
        }
        public bool responsive { get; set; }
        public bool maintainAspectRatio { get; set; }
        public Title title { get; set; }
        public Scales scales { get; set; }
    }
    public class Title
    {
        public Title()
        {
            text = string.Empty;
        }
        public bool display { get; set; }
        public string text { get; set; }
    }
    public class Scales
    {
        public Scales()
        {
            xAxes = new Axes();
            yAxes = new Axes();
        }
        public Axes xAxes { get; set; }
        public Axes yAxes { get; set; }
    }
    public class Axes
    {
        public Axes()
        {
            scaleLabel = new ScaleLabel();
            ticks = new Ticks();
        }
        public bool display { get; set; }
        public ScaleLabel scaleLabel { get; set; }
        public Ticks ticks { get; set; }
    }
    public class Ticks
    {
        public bool beginAtZero { get; set; }
    }
    public class ScaleLabel
    {
        public ScaleLabel()
        {
            labelString = string.Empty;
        }
        public bool show { get; set; }
        public string labelString { get; set; }
    }
}
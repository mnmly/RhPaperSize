using System;
using System.IO;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using Rhino.UI;
using Eto.Drawing;
using Eto.Forms;
using Newtonsoft.Json;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MNML
{
    public class RhPaperCommand : Rhino.Commands.Command
    {
        public override string EnglishName => "PaperSize";

        protected override Result RunCommand(Rhino.RhinoDoc doc, RunMode mode)
        {


            var dirname = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            string[] pathComponent = new string[] { dirname.ToString(), @"common_paper_sizes.json" };
            var jsonPath = Path.Combine(pathComponent);

            GetPoint gp = new GetPoint();
            string text = System.IO.File.ReadAllText(jsonPath);

            gp.SetCommandPrompt("Set Origin of Paper Rectangle");

            OptionToggle boolOption = new OptionToggle(false, "Off", "On");
            gp.AddOptionToggle("Portrait", ref boolOption);

            var listNames = new List<string>();
            var listValues = new List<Point2d>();



            List<Paper> papers = JsonConvert.DeserializeObject<List<Paper>>(text);
            //foreach(var paper in papers)
            //{
            var count = 0;
            foreach (var format in papers[0].formats)
            {
                var num = int.Parse(Regex.Match(format.name, @"\d+").Value);

                if (num < 6 && !format.name.Contains("C"))
                {
                    listNames.Add(format.name);
                    listValues.Add(new Point2d(format.size.mm[0], format.size.mm[1]));
                    count++;
                }

            }

            //}
            var listNamesArr = listNames.ToArray();
            int listIndex = 0;
            int opList = gp.AddOptionList("PaperType", listNamesArr, listIndex);

            while (true)
            {
                // perform the get operation. This will prompt the user to input a point, but also
                // allow for command line options defined above
                Rhino.Input.GetResult get_rc = gp.Get();
                if (gp.CommandResult() != Rhino.Commands.Result.Success)
                    return gp.CommandResult();

                if (get_rc == Rhino.Input.GetResult.Point)
                {
                    var plane = Plane.WorldXY;
                    plane.Origin = gp.Point();
                    var item = listValues[listIndex];
                    var isPortrait = boolOption.CurrentValue;
                    var rect = new Rectangle3d(Plane.WorldXY, isPortrait ? item.X : item.Y, isPortrait ? item.Y : item.X);
                    var attr = new Rhino.DocObjects.ObjectAttributes();
                    attr.Name = listNames[listIndex];
                    doc.Objects.AddRectangle(rect, attr);
                    doc.Views.Redraw();
                }
                else if (get_rc == Rhino.Input.GetResult.Option)
                {
                    if (gp.OptionIndex() == opList)
                    {
                        listIndex = gp.Option().CurrentListOptionIndex;
                    }
                    continue;
                }
                break;
            }
            return Rhino.Commands.Result.Success;

        }
    }
}
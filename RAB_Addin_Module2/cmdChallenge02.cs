using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.Exceptions;
using Autodesk.Revit.DB.Plumbing;
using System.Windows.Controls;
using Autodesk.Revit.DB;
using Microsoft.SqlServer.Server;

namespace RAB_Addin_Module2
{
    [Transaction(TransactionMode.Manual)]
    public class cmdChallenge02 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Revit application and document variables
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            // Your Module 02 Challenge code goes here
            //1.a pick single element
            Reference pickRef = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, "Select Element");
            Element pickElement = doc.GetElement(pickRef);

            //1.b Pick multiple elements
            List<Element> pickList = uidoc.Selection.PickElementsByRectangle("Select Element").ToList();

            TaskDialog.Show("Test", $"I selected {pickList.Count} elements.");

            // 2b. filter selected elements for model curves
            List<CurveElement> AllCurves = new List<CurveElement>();
            foreach (Element elem2 in pickList)
            {

                if (elem2 is CurveElement)
                {
                
                    AllCurves.Add(elem2 as CurveElement);
                    
                }

            }
            Level newLevel = Level.Create(doc, 0);

            //4 create a storefront wall type
            FilteredElementCollector StoreFrontWallTypes = new FilteredElementCollector(doc);
            StoreFrontWallTypes.OfCategory(BuiltInCategory.OST_Walls);
            StoreFrontWallTypes.WhereElementIsElementType();
            Curve curCurve1 = AllCurves[0].GeometryCurve;

            // need help here
            /*if (StoreFrontWallTypes.Name == "A-GLAZ")
            {

            }
            */
            
            //4b. create generic wall type
            FilteredElementCollector GenericWallTypes = new FilteredElementCollector(doc);
            GenericWallTypes.OfCategory(BuiltInCategory.OST_Walls);
            GenericWallTypes.WhereElementIsElementType();
            Curve curCurve2 = AllCurves[1].GeometryCurve;

            //6 get system types
            FilteredElementCollector systemCollector = new FilteredElementCollector(doc);
            systemCollector.OfClass(typeof(MEPSystem));

            //7. get duct system type
            MEPSystemType ductSystem = GetSystemTypeByName(doc, "M-DUCT");

            // 8 duct type
            FilteredElementCollector ductCollector = new FilteredElementCollector(doc);
            ductCollector.OfClass(typeof(DuctType));

            //9 create duct 
            Curve curCurve4 = AllCurves[2].GeometryCurve;

            //10. get pipe system type
            MEPSystemType pipeSystem = GetSystemTypeByName(doc, "P-PIPE");


            // 11 pipe type
            FilteredElementCollector pipeCollector = new FilteredElementCollector(doc);
            pipeCollector.OfClass(typeof(PipeType));

            //12 create pipe 
            Curve curCurve5 = AllCurves[3].GeometryCurve;
            Pipe newPipe = Pipe.Create(doc, pipeSystem.Id, pipeCollector.FirstElementId(), newLevel.Id, curCurve4.GetEndPoint(0), curCurve4.GetEndPoint(1));



            //5 create transaction with using statement
            using (Transaction t = new Transaction(doc))

            {
                t.Start();

                // 3. curve data
                foreach (CurveElement currentCurve in AllCurves)
                {
                    Curve curve = currentCurve.GeometryCurve;
                    XYZ startPoint = curve.GetEndPoint(0);
                    XYZ endPoint = curve.GetEndPoint(1);

                    GraphicsStyle curStyle = currentCurve.LineStyle as GraphicsStyle;

                    Debug.Print(curStyle.Name);

                    // 13. Switch statement

                    switch (curStyle.Name)
                    {
                        case "A-GLAZ":
                            Wall StorefrontWall = Wall.Create(doc, curCurve2, StoreFrontWallTypes.FirstElementId(), newLevel.Id, 0, 0, false, false);
                            break;

                        case "A-WALL":
                            Wall NewGenericWall = Wall.Create(doc, curCurve2, GenericWallTypes.FirstElementId(), newLevel.Id, 0, 0, false, false);
                            break;

                        case "M-Duct":
                            Duct newDuct = Duct.Create(doc, ductSystem.Id, ductCollector.FirstElementId(), newLevel.Id, curCurve4.GetEndPoint(0), curCurve4.GetEndPoint(1));
                            break;

                        case "P-Pipe":
                            newPipe = Pipe.Create(doc, pipeSystem.Id, pipeCollector.FirstElementId(), newLevel.Id, curCurve5.GetEndPoint(0), curCurve5.GetEndPoint(1));
                            break;

                    }

                }
                    
                t.Commit();

            }


            return Result.Succeeded;
        }

       internal MEPSystemType GetSystemTypeByName(Document doc, string name)
        {
            //6 get system types
            FilteredElementCollector systemCollector = new FilteredElementCollector(doc);
            systemCollector.OfClass(typeof(MEPSystem));

            //7. get duct system type
            foreach (MEPSystemType systemType in systemCollector)
            {
                if (systemType.Name == name)
                {
                    return systemType;
                }
            }

            return null;
        }   


       


  
                
internal static PushButtonData GetButtonData()
        {
            // use this method to define the properties for this command in the Revit ribbon
            string buttonInternalName = "btnChallenge02";
            string buttonTitle = "Module\r02";

            Common.ButtonDataClass myButtonData = new Common.ButtonDataClass(
                buttonInternalName,
                buttonTitle,
                MethodBase.GetCurrentMethod().DeclaringType?.FullName,
                Properties.Resources.Module02,
                Properties.Resources.Module02,
                "Module 02 Challenge");

            return myButtonData.Data;
        }
    }

}

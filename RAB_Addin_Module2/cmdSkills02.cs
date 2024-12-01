namespace RAB_Addin_Module2
{
    [Transaction(TransactionMode.Manual)]
    public class cmdSkills02 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Revit application and document variables
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            // Your Module 02 Skills code goes here
            // Delete the TaskDialog below and add your code
            TaskDialog.Show("Module 02 Skills", "Got Here!");
            //1.a pick single element
            Reference pickRef = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, "Select Element");
            Element pickElement = doc.GetElement(pickRef);

            //1.b Pick multiple elements
            List<Element> pickList = uidoc.Selection.PickElementsByRectangle("Select Element").ToList();

            TaskDialog.Show("Test", $"I selected {pickList.Count} elements.");

            // 2. filter selected elements for lines
            List<CurveElement> allCurves = new List<CurveElement>();
            foreach (Element elem in pickList)
            {
                if (elem is CurveElement)
                {
                    allCurves.Add(elem as CurveElement);
                }

            }

            // 2b. filter selected elements for model curves
            List<CurveElement> modelCurves = new List<CurveElement>();
            foreach (Element elem2 in pickList)
            {

                if (elem2 is CurveElement)
                {
                    CurveElement curveElem = elem2 as CurveElement;

                    if (curveElem.CurveElementType == CurveElementType.ModelCurve)
                    {
                        modelCurves.Add(curveElem);
                    }
                }

            }

            // 3. curve data
            foreach (CurveElement currentCurve in modelCurves)
            {
                Curve curve = currentCurve.GeometryCurve;
                XYZ startPoint = curve.GetEndPoint(0);
                XYZ endPoint = curve.GetEndPoint(1);

                GraphicsStyle curStyle = currentCurve.LineStyle as GraphicsStyle;
            }


            return Result.Succeeded;
        }
    }

}

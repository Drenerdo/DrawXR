using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEditor;
using UnityEngine;

public class DrawInputController : MonoBehaviour
{
    public GameObject lineObj;
    private GameObject curLine;
    public int colorNum = -1;
    public Color linecolor;

    private LineRenderer lineRenderer;
    private List<Vector3> linePosition;
    private List<GameObject> lines;
    private List<GameObject> lines_redo;

    public void ColorSelect(int num)
    {
        colorNum = num;

        if (colorNum == 0)
        {
            linecolor = Color.red;
        }
        else if (colorNum == 1)
        {
            linecolor = Color.green;
        }
        else if (colorNum == 2)
        {
            linecolor = Color.yellow;
        }
        else
        {
            linecolor = Color.white;
        }
    }

    void Awake()
    {
        colorNum = -1;
        linePosition = new List<Vector3>();
        lines = new List<GameObject>();
        lines_redo = new List<GameObject>();
    }

    void Update()
    {
        foreach (var controller in CoreServices.InputSystem.DetectedControllers)
        {
            if (controller.InputSource.SourceName == "Right Hand")
            {
                MixedRealityInteractionMapping[] inputMappings = controller.Interactions;
                MixedRealityInteractionMapping spatialInput = inputMappings[0];
                MixedRealityInteractionMapping selectInput = inputMappings[2];
                
                // Drawing only if you are selecting an object
                if (selectInput.BoolData)
                {
                    if (!curLine)
                    {
                        CreateLine(spatialInput.PositionData);
                        lines.Add(curLine);
                    }

                    if (Vector3.Distance(spatialInput.PositionData, linePosition[linePosition.Count - 1]) > 0.05f)
                    {
                        UpdateLine(spatialInput.PositionData);
                    }
                }
                else
                {
                    curLine = null;
                }
            }
        }
    }

    #region UpdateLine
    private void UpdateLine(Vector3 updatePosition)
    {
        linePosition.Add(updatePosition);
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, updatePosition);
    }
    #endregion

    #region CreateLine
    private void CreateLine(Vector3 initPosition)
    {
        curLine = Instantiate(lineObj, Vector3.zero, Quaternion.identity);
        lineRenderer = curLine.GetComponent<LineRenderer>();
        lineRenderer.material.color = linecolor;
        linePosition.Clear();
        linePosition.Add(initPosition);
        linePosition.Add(initPosition);
        
        lineRenderer.SetPosition(0, linePosition[0]);
        lineRenderer.SetPosition(1, linePosition[1]);
    }
    

    #endregion

    #region Undo Action
    public void UndoItem()
    {
        GameObject top_line = lines[lines.Count - 1];
        lines.RemoveAt(lines.Count - 1);
        top_line.SetActive(false);
        lines_redo.Add(top_line);
    }
    

    #endregion

    #region Redo Action
    public void RedoItem()
    {
        GameObject top_line = lines_redo[lines_redo.Count - 1];
        lines_redo.RemoveAt(lines_redo.Count - 1);
        top_line.SetActive(true);
        lines.Add(top_line);
    }
    #endregion
}

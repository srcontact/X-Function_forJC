using clrev01.Bases;
using clrev01.Extensions;
using clrev01.Programs.FieldPar;
using Shapes;
using System.Collections.Generic;
using UnityEngine;

namespace clrev01.PGE.PGBEditor.PGBEDetailMenu
{
    public abstract class Field3dIndicater<T> : BaseOfCL where T : IFieldEditObject
    {
        [SerializeField]
        protected LineRenderer origLine;
        [SerializeField]
        protected Shapes.Polyline origPolyLine;
        [SerializeField]
        protected float slerpRate = 0.5f;
        [SerializeField]
        protected T tgtFieldPar;
        private List<LineRenderer> _lines;
        private List<Polyline> _polyLines;
        protected abstract int lineNum { get; }

        private void Awake()
        {
            if (origLine != null)
            {
                _lines = new List<LineRenderer>();
                for (int i = 0; i < lineNum; i++)
                {
                    var lr = origLine.SafeInstantiate();
                    var t = lr.transform;
                    t.parent = transform;
                    t.localPosition = Vector3.zero;
                    t.localRotation = Quaternion.identity;
                    t.localScale = Vector3.one;
                    lr.gameObject.SetActive(true);
                    _lines.Add(lr);
                }
            }
            else if (origPolyLine != null)
            {
                _polyLines = new List<Polyline>();
                for (int i = 0; i < lineNum; i++)
                {
                    var lr = origPolyLine.SafeInstantiate();
                    var t = lr.transform;
                    t.SetParent(transform, false);
                    t.localPosition = Vector3.zero;
                    t.localRotation = Quaternion.identity;
                    t.localScale = Vector3.one;
                    lr.gameObject.SetActive(true);
                    _polyLines.Add(lr);
                }
            }
        }
        private void Update()
        {
            UpdateField();
        }
        protected abstract void UpdateField();
        protected void SetPoints(List<Vector3> points, int tgtNum)
        {
            if (_lines != null)
            {
                if (points == null) _lines[tgtNum].positionCount = 0;
                else
                {
                    _lines[tgtNum].positionCount = points.Count;
                    for (var i = 0; i < points.Count; i++)
                    {
                        var point = points[i];
                        _lines[tgtNum].SetPosition(i, point);
                    }
                }
            }
            else if (_polyLines != null)
            {
                if (points == null) _polyLines[tgtNum].gameObject.SetActive(false);
                else
                {
                    if (_polyLines[tgtNum].points.Count == points.Count)
                    {
                        for (int i = 0; i < points.Count; i++)
                        {
                            _polyLines[tgtNum].SetPointPosition(i, points[i]);
                        }
                    }
                    else
                    {
                        _polyLines[tgtNum].SetPoints(points);
                    }
                    _polyLines[tgtNum].gameObject.SetActive(true);
                }
            }
        }
        public virtual void SetIndicatePar(T fieldPar)
        {
            tgtFieldPar = fieldPar;
        }
    }
}
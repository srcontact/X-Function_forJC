using clrev01.Bases;
using clrev01.ClAction.Machines;
using clrev01.Extensions;
using clrev01.Menu;
using clrev01.Menu.Dialog;
using clrev01.PGE.PGB;
using clrev01.Programs;
using clrev01.Programs.FuncPar;
using clrev01.Programs.FuncPar.Comment;
using clrev01.Programs.FuncPar.FuncParType;
using clrev01.Save;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using static clrev01.Bases.UtlOfCL;
using static clrev01.Extensions.ExUI;
using static clrev01.Programs.UtlOfProgram;
using static I2.Loc.ScriptLocalization;

namespace clrev01.PGE.PGEM
{
    public partial class PGEManager : SingletonOfCL<PGEManager>, IScrollHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        private ComponentReferenceSet<PGBlock2> origPgb;
        [SerializeField]
        private MenuButton pgemMenuButton;
        public bool multiSelect;
        public PGBEditor.PGBEditor editMenu;

        #region nowEditPD

        [SerializeField]
        private CustomData _nowEditCD;
        public CustomData nowEditCD
        {
            get => _nowEditCD;
            private set
            {
                _nowEditCD = value;
                StaticInfo.Inst.UndoManager.nowMonitorPG = nowEditPD;
                pgbList = new List<PGBlock2>();
                PGBSetting();
            }
        }
        public PGData nowEditPD => nowEditCD.program;

        #endregion

        public CameraSizer uiCameraSizer;
        public QuickMenuDialog quickMenuDialog;
        public VariableEditor.VariableEditor variableEditor;
        public ClipBoard clipBoard;
        public PointerCursor pointerCursor;
        [HideInInspector]
        public List<PGBlock2> pgbList = new();

        #region currentClickedPGB

        public PGBlock2 currentClickedPGB => selectPgbs.Count > 0 ? selectPgbs.Last() : null;

        #endregion

        public List<PGBlock2> selectPgbs = new();
        [SerializeField, ReadOnly]
        private bool nowDragMove;
        public PGBData nowConnectionChangingPGB { get; set; }
        public int connectNum { get; set; }
        [SerializeField]
        PGBView.PGBView pGBView;
        [SerializeField]
        private float selectRadius = 50;
        [SerializeField]
        private RectTransform selectCircle;
        [SerializeField]
        private TextMeshProUGUI nodeCountText;
        [NonSerialized]
        public List<ConnectButton> connectButtons = new();

        public override void Awake()
        {
            base.Awake();
            pgemMenuButton.OnClick.AddListener(() => ExeOnPointerClick());
            pgemMenuButton.OnRightClick.AddListener(() => ExeOnRightClick());
            pgemMenuButton.OnBeginDrag += (PointerEventData e) => ExeOnBeginDrag(e);
            pgemMenuButton.OnDrag += (PointerEventData e) => ExeOnDrag(e);
            pgemMenuButton.OnEndDrag += (PointerEventData e) => ExeOnEndDrag(e);
            pgemMenuButton.OnDrop += (PointerEventData e) => ExeOnDrop(e);
            var sd = selectCircle.sizeDelta;
            sd.x = sd.y = selectRadius * 2;
            selectCircle.sizeDelta = sd;
            selectCircle.gameObject.SetActive(false);
            InitScroll();
        }
        private void Start()
        {
            ConnectButtonActiveUpdate();
        }
        private void OnEnable()
        {
            nowEditCD = StaticInfo.Inst.nowEditMech;
        }
        private void OnDisable()
        {
            for (int i = 0; i < pgbList.Count; i++)
            {
                PoolingPGB(i);
            }
        }

        public void PGBSetting()
        {
            if (pgbList.Count <= nowEditPD.pgList.Count)
            {
                pgbList.AddRange(Enumerable.Repeat<PGBlock2>(null, nowEditPD.pgList.Count - pgbList.Count));
            }
            for (int i = 0; i < nowEditPD.pgList.Count; i++)
            {
                if (nowEditPD.pgList[i] == null && pgbList[i] != null)
                {
                    PoolingPGB(i);
                }
                else
                {
                    if (nowEditPD.pgList[i] != null && nowEditPD.pgList[i].funcPar is GetTargetStatusValueFuncPar) Debug.Log(nowEditPD.pgList[i].funcPar.GetType());
                    if (nowEditPD.pgList[i] != null && pgbList[i] == null) LoadPGB(i);
                    if (pgbList[i] != null)
                    {
                        LoadNodeFace(pgbList[i]);
                        pgbList[i].DataSetting();
                    }
                }
            }
            SetRoutineNum();
            pGBView.viewPGData = nowEditPD;
            NodeCount();
        }

        private PGBlock2 LoadPGB(int index)
        {
            var nb = origPgb.GetInstanceUsePool(out _);
            nb.EditPg = nowEditPD;
            pgbList[index] = nb;
            nb.index = index;
            nb.gameObject.transform.SetParent(blocks);
            nb.gameObject.SetActive(true);
            return nb;
        }

        private void PoolingPGB(int index)
        {
            var pgb = pgbList[index];
            if (pgb == null) return;
            PoolingNodeFace(pgb);
            pgb.gameObject.SetActive(false);
            pgb.objectPool.PoolingObject(pgb);
            pgbList[index] = null;
        }

        private void LoadNodeFace(PGBlock2 pgb)
        {
            var type = pgb.funcPar.GetType();
            var key = NfHub.GetNodeKey(type);
            if (pgb.nodeFace != null && pgb.nodeFace.key == key) return;
            if (pgb.nodeFace != null)
            {
                PoolingNodeFace(pgb);
            }
            pgb.nodeFace = NfHub.GetNodeFaceData(type).GetNodeFace(gameObject);
            pgb.nodeFace.gameObject.SetActive(true);
            pgb.nodeFace.transform.SetParent(pgb.nodeFaceArea);
            pgb.rectTransform.sizeDelta = pgb.nodeFace.rectTransform.sizeDelta;
        }

        private void PoolingNodeFace(PGBlock2 pgb)
        {
            var nodeFace = pgb.nodeFace;
            nodeFace.transform.SetParent(blocks);
            nodeFace.gameObject.SetActive(false);
            nodeFace.objectPool.PoolingObject(nodeFace);
            pgb.nodeFace = null;
        }

        public void SetRoutineNum()
        {
            foreach (var pgb in pgbList)
            {
                if (pgb == null) continue;
                pgb.editorPar.routineNum = -1;
            }

            var addLog = new List<int>();
            //スタート、ルーチンルートから繋がっているノードのルーチン番号を判定
            var startRoutineRoots = pgbList.Where(x =>
            {
                if (x == null) return false;
                var type = x.funcPar.GetType();
                return !x.connected && (type == typeof(StartFuncPar) || type == typeof(SubroutineRootFuncPar));
            }).ToList().ConvertAll(x => x.index);
            foreach (var routineRoot in startRoutineRoots)
            {
                addLog.Clear();
                RecursiveSetRoutineNum(routineRoot, routineRoot, addLog);
            }

            //スタート、ルーチンルート以外のどこからも繋がっていないノードから繋がっているノードのルーチン番号を判定
            var notStartRoutineRoots = pgbList.Where(x =>
            {
                if (x == null) return false;
                var type = x.funcPar.GetType();
                return !x.connected && type != typeof(StartFuncPar) && type != typeof(SubroutineRootFuncPar);
            }).ToList().ConvertAll(x => x.index);
            foreach (var routineRoot in notStartRoutineRoots)
            {
                addLog.Clear();
                var res = RecursiveGetRoutineNumNotConnectedToStart(routineRoot, addLog);
                if (res > -1)
                {
                    foreach (var index in addLog)
                    {
                        pgbList[index].editorPar.routineNum = res;
                    }
                }
            }
        }

        private void RecursiveSetRoutineNum(int index, int routineNum, ICollection<int> addLog)
        {
            if (index < 0 || addLog.Contains(index)) return;
            addLog.Add(index);
            var pgb = pgbList[index];
            pgb.editorPar.routineNum = routineNum;
            RecursiveSetRoutineNum(pgb.nextIndex, routineNum, addLog);
            RecursiveSetRoutineNum(pgb.falseNextIndex, routineNum, addLog);
        }

        private int RecursiveGetRoutineNumNotConnectedToStart(int index, ICollection<int> addLog)
        {
            if (index < 0 || addLog.Contains(index)) return -1;
            addLog.Add(index);
            var pgb = pgbList[index];
            var np = pgb.NextPgb;
            if (np != null && np.editorPar.routineNum > -1) return np.editorPar.routineNum;
            var fnp = pgb.FalseNextPgb;
            if (fnp != null && fnp.editorPar.routineNum > -1) return fnp.editorPar.routineNum;
            var next = RecursiveGetRoutineNumNotConnectedToStart(pgb.nextIndex, addLog);
            var falseN = RecursiveGetRoutineNumNotConnectedToStart(pgb.falseNextIndex, addLog);
            if (next > -1) return next;
            if (falseN > -1) return falseN;
            return -1;
        }

        private void NodeCount()
        {
            nodeCountText.SetText("{0}", nowEditPD.pgList.Count(x => x != null));
        }

        private void Update()
        {
            pGBView.RunOnUpdate();
        }

        public void CreateNewPGD(Vector3 nodePosition, Vector3 qmPosition, PGBData connectionRoot = null, int next = -1, int fNext = -1)
        {
            UniTask.Create(async () =>
            {
                await UniTask.DelayFrame(1);
                OpenPgbTypeSelector(qmPosition, i =>
                {
                    var addablePgbType = PGBFuncPar.AddablePGBTypes[i];
                    ExeAddNewPgbd(nodePosition, connectionRoot, next, fNext, addablePgbType);
                });
            }).Forget();
        }
        private void AddCommentPgbd(Vector3 nodePosition)
        {
            UniTask.Create(async () =>
            {
                await UniTask.DelayFrame(1);
                ExeAddNewPgbd(nodePosition, null, -1, -1, typeof(CommentFuncPar));
            }).Forget();
        }
        private void ExeAddNewPgbd(Vector3 nodePosition, PGBData connectionRoot, int next, int fNext, Type pgbdType)
        {
            var isValid = StaticInfo.Inst.UndoManager.UpdatePgbdStart();
            var nd = nowEditPD.CreatePGBD(pgbdType, nodePosition, next, fNext);
            PGBSetting();
            var pgb = pgbList[nd.editorPar.myIndex];
            pgb.ConnectionChanging(connectionRoot, PGEM2.connectNum);
            pgb.SelectThisSimple();
            if (pgb.funcPar.OpenEditorOnCreateNode)
            {
                pgb.EditGo(() =>
                {
                    StaticInfo.Inst.UndoManager.UpdatePgbdLog(isValid);
                    StaticInfo.Inst.UndoManager.UpdatePgbdEnd(isValid);
                });
            }
            else
            {
                StaticInfo.Inst.UndoManager.UpdatePgbdLog(isValid);
                StaticInfo.Inst.UndoManager.UpdatePgbdEnd(isValid);
            }
        }

        public void PastePGDs(Vector3 position, List<PGBData> origList)
        {
            nowEditPD.PastePGBDs(position, origList);
            PGBSetting();
        }

        public void DeleteExe(List<PGBlock2> deleteList)
        {
            Debug.Log("DeleteExe");
            nowEditPD.DeletePGBDs(deleteList.ConvertAll(x => x.pgbd));
            deleteList.Clear();
            PGEM2.PGBSetting();
        }


        private void ExeOnPointerClick()
        {
            if (nowDragMove || doTouchScaling) return;
            var selected = currentClickedPGB != null;
            ResetSelectPgbs();
            if (currentClickedPGB == null && !editMenu.isActiveAndEnabled)
            {
                pointerCursor.SetPosition();
                if (!selected) SelectOnClickBackground();
            }
            editMenu.CloseEditor(false);
            if (nowConnectionChangingPGB != null)
            {
                OpenConnectQuickMenuOnClickBackground(nowConnectionChangingPGB);
                nowConnectionChangingPGB = null;
            }
        }

        private void SelectOnClickBackground()
        {
            var selected = GetPgbInRadius();
            if (selected != null) selected.SelectThisNormalClick();
        }

        public void OpenConnectQuickMenuOnClickBackground(PGBData connectionChangingPgbStore)
        {
            quickMenuDialog.OpenQuickMenu(new List<string> { pgEditor.createNew }, i => { ConnectModeQuickMenu(i, connectionChangingPgbStore); });
        }

        private void ConnectModeQuickMenu(int i, PGBData connectionChangingPgbStore, PointerEventData eventData = null)
        {
            switch (i)
            {
                case 0:
                    CreateNewPGD(pointerCursor.lpos, Camera.main.WorldToScreenPoint(PGEM2.pointerCursor.pos), connectionChangingPgbStore);
                    break;
            }
        }

        public PGBlock2 GetPgbInRadius()
        {
            var selectRadiusReal = selectRadius / nowScale;
            var localPointerPos = blocks.InverseTransformPoint(GetPointerPos());
            localPointerPos.z = 0;
            PGBlock2 selected = null;
            float selectedDist = float.MaxValue;
            foreach (var pgb in pgbList)
            {
                if (pgb == null) continue;
                var dist = Vector3.Distance(pgb.lpos, localPointerPos);
                if (dist > selectRadiusReal) continue;
                if (dist < selectedDist)
                {
                    selected = pgb;
                    selectedDist = dist;
                }
            }

            return selected;
        }

        public void SelectAllDownstream()
        {
            if (currentClickedPGB == null || (currentClickedPGB.NextPgb == null && currentClickedPGB.FalseNextPgb == null)) return;
            multiSelect = true;
            var current = currentClickedPGB;
            var selected = new List<PGBlock2>();
            RecursiveSelectDownstream(currentClickedPGB, selected);
            for (var i = selected.Count - 1; i >= 0; i--)
            {
                selected[i].SelectThisOtherActions();
            }
            current.ReselectThis();
        }

        private void RecursiveSelectDownstream(PGBlock2 selectRoot, List<PGBlock2> selected)
        {
            selected.Add(selectRoot);
            if (selectRoot.NextPgb != null && !selected.Contains(selectRoot.NextPgb))
            {
                RecursiveSelectDownstream(selectRoot.NextPgb, selected);
            }
            if (selectRoot.FalseNextPgb != null && !selected.Contains(selectRoot.FalseNextPgb))
            {
                RecursiveSelectDownstream(selectRoot.FalseNextPgb, selected);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            ShowSelectCircle();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            HideSelectCircle();
        }

        public void ShowSelectCircle()
        {
            selectCircle.position = GetPointerLocalPos;
            selectCircle.gameObject.SetActive(true);
        }

        public void HideSelectCircle()
        {
            selectCircle.gameObject.SetActive(false);
        }

        public void MoveSelectCircle()
        {
            selectCircle.position = GetPointerLocalPos;
        }

        private void ExeOnRightClick()
        {
            if (nowDragMove || doTouchScaling) return;
            pointerCursor.SetPosition();
            var selected = currentClickedPGB != null;
            var menuTexts = selected ? new List<string> { pgEditor.copy, pgEditor.cut, pgEditor.delete } : new List<string> { pgEditor.createNew, pgEditor.paste, pgEditor.addComment };
            quickMenuDialog.OpenQuickMenu(menuTexts, (i) => RightClickBackground(i, selected));
        }

        private void RightClickBackground(int i, bool selected)
        {
            if (selected)
            {
                switch (i)
                {
                    case 0:
                        clipBoard.CopySelected();
                        multiSelect = false;
                        ResetSelectPgbs();
                        break;
                    case 1:
                        clipBoard.CutSelected();
                        multiSelect = false;
                        ResetSelectPgbs();
                        break;
                    case 2:
                        DeleteExe(selectPgbs);
                        multiSelect = false;
                        break;
                }
            }
            else
            {
                switch (i)
                {
                    case 0:
                        CreateNewPGD(pointerCursor.lpos, Camera.main.WorldToScreenPoint(PGEM2.pointerCursor.pos));
                        PGBSetting();
                        break;
                    case 1:
                        PastePGDs(pointerCursor.lpos, clipBoard.clipedPGDs);
                        PGBSetting();
                        break;
                    case 2:
                        AddCommentPgbd(pointerCursor.lpos);
                        PGBSetting();
                        break;
                }
            }
        }

        public void ResetSelectPgbs()
        {
            for (int i = selectPgbs.Count - 1; i >= 0; i--)
            {
                selectPgbs[i].DeselectThis();
            }
        }

        public void MoveTrackPointer(Transform pointerT)
        {
            var v = GetPointerLocalPos;
            pointerT.position = v;
        }
        private Vector3 GetPointerLocalPos
        {
            get
            {
                var v = GetPointerPos();
                v.z = PGEM2.pos.z;
                return v;
            }
        }

        private void ExeOnBeginDrag(PointerEventData eventData)
        {
            nowDragMove = true;
            DragMoveStart();
            HideSelectCircle();
        }

        private void ExeOnDrag(PointerEventData eventData)
        {
            DragMoveExe();
        }

        private void ExeOnEndDrag(PointerEventData eventData)
        {
            UniTask.Create(async () =>
            {
                await UniTask.DelayFrame(1);
                if (Input.touchCount <= 0 && !Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2))
                {
                    nowDragMove = false;
                    doTouchScaling = false;
                }
            }).Forget();
        }

        public void OnScroll(PointerEventData eventData)
        {
            ScrollScaling();
        }

        private void ExeOnDrop(PointerEventData eventData)
        { }

        public void OpenPgbTypeSelector(Vector3 openPos, Action<int> action)
        {
            quickMenuDialog.OpenQuickMenu(
                PGBFuncPar.AddableTypeStrings, i => action(i), openPos, PGBFuncPar.ColorBlockAssets);
        }

        public void ConnectButtonActiveUpdate()
        {
            foreach (var connectButton in connectButtons)
            {
                connectButton.ActiveUpdate();
            }
        }
    }
}
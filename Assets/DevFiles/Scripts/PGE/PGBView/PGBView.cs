using clrev01.Bases;
using clrev01.ClAction;
using clrev01.Programs.FuncPar;
using clrev01.Programs.FuncPar.FuncParType;
using clrev01.Save;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering; // ShadowCastingMode など

namespace clrev01.PGE.PGBView
{
    public class PGBView : BaseOfCL, IRunner
    {
        [SerializeField]
        Camera programViewCamera;

        [SerializeField]
        List<Material> blockMaterials = new List<Material>();

        [SerializeField]
        List<Material> blockHighlightMaterials = new List<Material>();

        [SerializeField]
        List<Material> lineMaterials = new List<Material>();

        [SerializeField]
        List<Material> lineHighLightMaterials = new List<Material>();

        [SerializeField]
        Vector3 lineOffset = new Vector3(0.2f, 0, 1);

        float editorPosMagnitude = 0.002f * 8;

        Mesh blockMesh, lineMesh;
        int tgtLayer;
        int currentUpdateActionFrame = int.MinValue;

        [NonSerialized]
        public PGData viewPGData;

        public List<PGBData> ViewPGBDList
        {
            get
            {
                if (viewPGData != null) return viewPGData.pgList;
                return ActionManager.Inst != null ? ActionManager.Inst.machineList[ActionManager.Inst.cameraTgtMachine].ld.pgExeData.copiedPGData.pgList : null;
            }
        }

        public Bounds PgbdBounds
        {
            get
            {
                if (viewPGData != null)
                {
                    return viewPGData.CalcPgbdBounds();
                }
                else if (ActionManager.Inst != null || ActionManager.Inst.machineList[ActionManager.Inst.cameraTgtMachine] != null)
                {
                    return ActionManager.Inst.machineList[ActionManager.Inst.cameraTgtMachine].ld.pgExeData.programBounds;
                }
                else
                {
                    return new Bounds();
                }
            }
        }

        private bool _isActionScene;


        private void Awake()
        {
            tgtLayer = LayerMask.NameToLayer("ProgramView");
            CreateBlockMesh();
            CreateLineMesh();
        }

        private void OnEnable()
        {
            _isActionScene = ActionManager.Inst;
        }

        private void CreateBlockMesh()
        {
            blockMesh = new Mesh();
            blockMesh.vertices = new Vector3[]
            {
                new Vector3(0.5f, 0.5f, 0),
                new Vector3(-0.5f, 0.5f, 0),
                new Vector3(0.5f, -0.5f, 0),
                new Vector3(-0.5f, -0.5f, 0),
            };
            blockMesh.uv = new Vector2[]
            {
                new Vector2(0, 0),
                new Vector2(1f, 0),
                new Vector2(1f, 1f),
                new Vector2(0, 1f),
            };
            blockMesh.triangles = new int[]
            {
                1, 0, 2,
                1, 2, 3,
            };
        }

        private void CreateLineMesh()
        {
            lineMesh = new Mesh();
            lineMesh.vertices = new Vector3[]
            {
                new Vector3(0.05f, 1f, 0) + lineOffset,
                new Vector3(-0.05f, 1f, 0) + lineOffset,
                new Vector3(0.15f, 0f, 0) + lineOffset,
                new Vector3(-0.15f, 0f, 0) + lineOffset,
            };
            lineMesh.uv = new Vector2[]
            {
                new Vector2(0, 0),
                new Vector2(1f, 0),
                new Vector2(1f, 1f),
                new Vector2(0, 1f),
            };
            lineMesh.triangles = new int[]
            {
                1, 0, 2,
                1, 2, 3,
            };
        }

        public void RunAfterPhysics()
        { }

        public void RunBeforePhysics()
        { }

        public void RunOnUpdate()
        {
            Bounds tgtBounds = PgbdBounds;
            programViewCamera.transform.position =
                tgtBounds.center * editorPosMagnitude + Vector3.back;
            programViewCamera.orthographicSize =
                Mathf.Max(tgtBounds.extents.x, tgtBounds.extents.y) * editorPosMagnitude + 1;
            UpdateView();
        }

        private void UpdateView()
        {
            // アクションシーン時のフレーム同期
            if (_isActionScene && currentUpdateActionFrame != ActionManager.Inst.actionFrame)
            {
                currentUpdateActionFrame = ActionManager.Inst.actionFrame;
            }

            // ノード（ブロック）用とライン用で、それぞれ
            // Material -> List<Matrix4x4> を保持する辞書を作る
            Dictionary<Material, List<Matrix4x4>> blockMatrices = new Dictionary<Material, List<Matrix4x4>>();
            Dictionary<Material, List<Matrix4x4>> lineMatrices  = new Dictionary<Material, List<Matrix4x4>>();

            // ノード描画分をまとめる
            foreach (var data in ViewPGBDList)
            {
                if (data == null) continue;

                // ブロックを描画するかどうか判定
                Vector3 bp = ((Vector3)data.editorPar.EditorPos) * editorPosMagnitude;
                bool isExecutedAfterCurrentFrame = data.currentExecutedFrame >= currentUpdateActionFrame;

                // ノードの種類に応じてマテリアルを決定
                Material mat;
                switch (data.funcPar)
                {
                    case StartFuncPar x:
                        mat = isExecutedAfterCurrentFrame ? blockHighlightMaterials[0] : blockMaterials[0];
                        break;
                    case ActionFuncPar x:
                        mat = isExecutedAfterCurrentFrame ? blockHighlightMaterials[1] : blockMaterials[1];
                        break;
                    case BranchFuncPar x:
                        mat = isExecutedAfterCurrentFrame ? blockHighlightMaterials[2] : blockMaterials[2];
                        break;
                    case FunctionFuncPar x:
                        mat = isExecutedAfterCurrentFrame ? blockHighlightMaterials[3] : blockMaterials[3];
                        break;
                    case SubroutineFuncPar x:
                        mat = isExecutedAfterCurrentFrame ? blockHighlightMaterials[4] : blockMaterials[4];
                        break;
                    default:
                        mat = isExecutedAfterCurrentFrame ? blockHighlightMaterials[0] : blockMaterials[0];
                        break;
                }

                // ブロック用の変換行列
                Matrix4x4 blockMatrix = Matrix4x4.TRS(bp, Quaternion.identity, Vector3.one);
                AddMatrix(blockMatrices, mat, blockMatrix);

                // 次のノードがあればラインも登録
                if (data.editorPar.nextIndex != -1)
                {
                    DrawConnectLineInstanced(data.editorPar.nextIndex, bp, 0, isExecutedAfterCurrentFrame, lineMatrices);
                }
                // falseNextIndex があればラインも登録
                if (data.editorPar.falseNextIndex != -1)
                {
                    DrawConnectLineInstanced(data.editorPar.falseNextIndex, bp, 1, isExecutedAfterCurrentFrame, lineMatrices);
                }
            }

            // ↑で辞書にためた分を一括描画
            DrawMeshInstancedAll(blockMesh, blockMatrices);
            DrawMeshInstancedAll(lineMesh, lineMatrices);
        }

        /// <summary>
        /// DrawMeshInstanced 用に、辞書に Matrix4x4 を登録するヘルパー
        /// </summary>
        private void AddMatrix(Dictionary<Material, List<Matrix4x4>> dict, Material mat, Matrix4x4 matrix)
        {
            if (!dict.ContainsKey(mat))
            {
                dict[mat] = new List<Matrix4x4>();
            }
            dict[mat].Add(matrix);
        }

        /// <summary>
        /// ノード同士を結ぶ線の情報をまとめるためのメソッド
        /// </summary>
        private void DrawConnectLineInstanced(int nextNum, Vector3 bp, int matNum, bool blockIsExecuted,
            Dictionary<Material, List<Matrix4x4>> lineMatrices)
        {
            var next = ViewPGBDList[nextNum];
            // 次のノードが「現在のフレーム以降に実行される」ブロックであればハイライト
            bool isExecutedAfterCurrentFrame = blockIsExecuted && next.currentExecutedFrame >= currentUpdateActionFrame;
            Vector3 nbp = ((Vector3)next.editorPar.EditorPos) * editorPosMagnitude;
            Quaternion lineRot = Quaternion.LookRotation(Vector3.forward, nbp - bp);
            float len = Vector3.Distance(nbp, bp);

            // ラインの変換行列
            Matrix4x4 matrix = Matrix4x4.TRS(bp, lineRot, new Vector3(1, len, 1));

            // ライン用マテリアルを選択
            Material lineMat = isExecutedAfterCurrentFrame ?
                lineHighLightMaterials[matNum] : lineMaterials[matNum];

            // 辞書に登録
            AddMatrix(lineMatrices, lineMat, matrix);
        }

        /// <summary>
        /// Dictionary にまとめた描画情報をまとめて DrawMeshInstanced する
        /// </summary>
        private void DrawMeshInstancedAll(Mesh mesh, Dictionary<Material, List<Matrix4x4>> dict)
        {
            foreach (var kv in dict)
            {
                Material mat = kv.Key;
                List<Matrix4x4> matrixList = kv.Value;

                // Graphics.DrawMeshInstanced の上限 1023 個ごとに分割して描画
                for (int i = 0; i < matrixList.Count; i += 1023)
                {
                    int count = Mathf.Min(1023, matrixList.Count - i);

                    // 必要分だけ切り出して配列に変換
                    Matrix4x4[] subMatrices = new Matrix4x4[count];
                    matrixList.CopyTo(i, subMatrices, 0, count);

                    Graphics.DrawMeshInstanced(
                        mesh,
                        0,           // サブメッシュのインデックス
                        mat,
                        subMatrices,
                        count,
                        null,        // MaterialPropertyBlock (使用しない場合は null)
                        ShadowCastingMode.Off, // 影は落とさない想定
                        false,       // 受けるかどうか
                        tgtLayer
                    );
                }
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class Enemy : MapObjectBase
{
    public virtual void MoveStart()
    {
        var player = UnityEngine.Object.FindObjectOfType<Player>();
        if (!MoveToFollow(player))
        {
            MoveFree();
        }
    }

    [System.Serializable]
    public class Scope
    {
        // South方向
        [TextArea(3, 10)]
        public string Area = ""
            + "111\n"
            + "111\n"
            + "111";

        /// <summary>
        /// 相手が視野の中にいるかを判定する
        /// </summary>
        /// <param name="target"></param>
        /// <param name="startPos"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        public bool IsInArea(Vector2Int target, Vector2Int startPos, Direction dir)
        {
            var relativePos = target - startPos;
            switch (dir)
            {
                case Direction.North:
                    relativePos.x *= -1;
                    relativePos.y *= -1;
                    break;
                case Direction.South:
                    break;
                case Direction.East:
                    var tmp = relativePos.x;
                    relativePos.x = -relativePos.y;
                    relativePos.y = tmp;
                    break;
                case Direction.West:
                    tmp = relativePos.x;
                    relativePos.x = relativePos.y;
                    relativePos.y = -tmp;
                    break;
            }

            var lines = Area.Split('\n');
            var width = lines.Select(_l => _l.Length).FirstOrDefault();
            if (!lines.All(_l => _l.Length == width))
            {
                throw new System.Exception("Areaの各行にサイズが異なるものが存在しています。");
            }

            var left = -width / 2;
            var right = left + width;
            if (left <= relativePos.x && relativePos.x < right)
            {
                if (1 <= relativePos.y && relativePos.y <= lines.Length)
                {
                    var offsetX = relativePos.x - left;
                    if ('1' == lines[relativePos.y - 1][offsetX])
                    {
                        return true;
                    }
                }
            }
            return false;

        }
    }

    public bool IsChasing = false;  // 追いかけているか？
    public Scope VisibleArea;       // 視野

    protected bool MoveToFollow(MapObjectBase target)
    {
        if (VisibleArea.IsInArea(target.Pos, Pos, Forward))
        {
            Move(Forward);
            IsChasing = true;
            return true;
        }

        // 追跡中にプレイヤーが視野の範囲外に行ってしまった場合は、
        // 現在の前方向からの左右を確認し、その中にプレイヤーがいるか判定する
        if (IsChasing)
        {
            var left = Map.TurnLeftDirection(Forward);
            if (VisibleArea.IsInArea(target.Pos, Pos, left))
            {
                Move(Forward);
                Forward = left;
                IsChasing = true;
                return true;
            }
            var right = Map.TurnRightDirection(Forward);
            if (VisibleArea.IsInArea(target.Pos, Pos, right))
            {
                Move(Forward);
                Forward = right;
                IsChasing = true;
                return true;
            }
        }

        IsChasing = false;
        return false;
    }



    private void MoveFree()
    {
        // 現在の左方向から順に右回りに進めるマスか確認していく
        var startDir = Map.TurnLeftDirection(Forward);
        Forward = startDir;
        do
        {
            //- 壁だと移動できない
            //- 既に他のマップオブジェクトが存在している場合は移動できない
            var (movedMass, movedPos) = Map.GetMovePos(Pos, Forward);
            var massData = movedMass == null ? null : Map[movedMass.Type];
            if (movedMass == null || movedMass.ExistObject != null || !massData.IsRoad)
            {
                // 移動できなければ向きを変え、移動確認を行う
                Forward = Map.TurnRightDirection(Forward);
            }
            else
            {
                // 移動可能
                break;
            }
        } while (startDir != Forward);

        // 移動の前方向を決定したら移動する
        Move(Forward);
    }
}

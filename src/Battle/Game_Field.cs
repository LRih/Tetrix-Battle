using System;
using System.Linq;
using System.Collections.Generic;
using SwinGame;
using Color = System.Drawing.Color;

namespace TetrixBattle.src
{
    //#==============================================================
    //# * Game_Field
    //#==============================================================
    public class Game_Field
    {
        //#----------------------------------------------------------
        //# * Constants
        //#----------------------------------------------------------
        private const int REINFORCEMENT_WIDTH = 5;
        private const int BATTLEZONE_WIDTH = 16;
        private const int BATTLEFIELD_GRID_WIDTH = BATTLEZONE_WIDTH + (REINFORCEMENT_WIDTH * 2);
        private const int BATTLEFIELD_GRID_HEIGHT = 7;
        private const int ENEMY_REINFORCEMENT_START_X = BATTLEFIELD_GRID_WIDTH - REINFORCEMENT_WIDTH;
        public readonly int START_X;
        public readonly int START_Y;
        //#----------------------------------------------------------
        //# * Variables
        //#----------------------------------------------------------
        public int UnitsLost; // keep track of player units lost
        private Game_BattleUnit[,] _units = new Game_BattleUnit[BATTLEFIELD_GRID_WIDTH, BATTLEFIELD_GRID_HEIGHT]; // 2d array for storing all units on the battlefield
        private List<Effect_Missile> _effectMissiles = new List<Effect_Missile>();
        private List<Effect> _effects = new List<Effect>(); // holds text / hit effects
        private List<string> _playedSounds = new List<string>(); // hold sounds already played this frame
        //#----------------------------------------------------------
        //# * Initialize
        //#----------------------------------------------------------
        public Game_Field(int x, int y)
        {
            START_X = x;
            START_Y = y;
        }
        //#----------------------------------------------------------
        //# * Update
        //#----------------------------------------------------------
        public void Update()
        {
            ClearSoundFlags();
            UpdatePreAction();
            UpdateAura();
            UpdateJump();
            UpdateAction();
            UpdateMissiles();
            UpdateMovement();
            UpdateEffects();
        }
        private void ClearSoundFlags()
        {
            _playedSounds.Clear();
        }
        private void UpdatePreAction()
        {
            foreach (Point2DEx pt in GetUnitPts())
            {
                Game_BattleUnit unit = _units[pt.X, pt.Y];
                if (unit != null)
                {
                    unit.Update();
                    unit.ClearTurnFlags();
                    if (unit.IsDead() && unit.IsReadyToDespawn()) _units[pt.X, pt.Y] = null;
                }
            }
        }
        private void UpdateAura()
        {
            foreach (Point2DEx pt in GetUnitPts())
            {
                Game_BattleUnit unit = _units[pt.X, pt.Y];
                if (unit != null && !unit.IsDead()) ProcessAura(pt);
            }
        }
        private void UpdateJump()
        {
            for (int y = 0; y < BATTLEFIELD_GRID_HEIGHT; y++)
            {
                // jump player units
                for (int x = GetFrontMostUnitX(y, true); x >= 0; x--) ProcessJump(new Point2DEx(x, y));
                // jump enemy units
                for (int x = GetFrontMostUnitX(y, false); x < BATTLEFIELD_GRID_WIDTH; x++) ProcessJump(new Point2DEx(x, y));
            }
        }
        private void UpdateAction()
        {
            foreach (Point2DEx pt in GetUnitPts())
            {
                Game_BattleUnit unit = _units[pt.X, pt.Y];
                if (unit != null)
                {
                    ProcessProjectileSkill(pt);
                    ProcessMagicSkill(pt);
                    ProcessAttack(pt);
                }
            }
        }
        private void UpdateMissiles()
        {
            // remove landed arrows using backwards iteration
            for (int missileId = _effectMissiles.Count - 1; missileId >= 0; missileId--)
            {
                bool hit = false;
                Effect_Missile missile = _effectMissiles[missileId];
                foreach (Point2DEx targetPt in missile.GetCurrentTargetPts())
                {
                    if (!IsValidPt(targetPt)) continue;
                    // if splash missile, show hit animation and play sound even if no target
                    if (missile.IsSplash())
                    {
                        _effects.Add(new Effect_Bitmap(missile.DamageAgent.BitmapHit, START_X + targetPt.X * 40, START_Y + targetPt.Y * 40));
                        PlaySound(missile.DamageAgent.SoundHit);
                    }
                    Game_Unit targetUnit = GetTarget(targetPt);
                    if (missile.IsLegalAttackTarget(targetUnit))
                    {
                        DamageHandling(missile.DamageAgent, targetPt);
                        hit = true;
                        if (!missile.IsSplash()) _effects.Add(new Effect_Bitmap(missile.DamageAgent.BitmapHit, START_X + targetPt.X * 40, START_Y + targetPt.Y * 40));
                    }
                }
                if (missile.IsExpired() || hit) _effectMissiles.RemoveAt(missileId);
                else missile.Update();
            }
        }
        private void UpdateMovement()
        {
            for (int y = 0; y < BATTLEFIELD_GRID_HEIGHT; y++)
            {
                // move player units
                for (int x = GetFrontMostUnitX(y, true); x >= 0; x--) ProcessMovement(new Point2DEx(x, y));
                // move enemy units
                for (int x = GetFrontMostUnitX(y, false); x < BATTLEFIELD_GRID_WIDTH; x++) ProcessMovement(new Point2DEx(x, y));
            }
        }
        private void UpdateEffects()
        {
            for (int i = _effects.Count - 1; i >= 0; i--)
            {
                if (_effects[i].IsExpired()) _effects.RemoveAt(i);
                else _effects[i].Update();
            }
        }
        //#----------------------------------------------------------
        //# * Update Draw
        //#----------------------------------------------------------
        public void UpdateDraw()
        {
            DrawUnits();
            DrawEffects();
        }
        private void DrawUnits()
        {
            foreach (Point2DEx pt in GetUnitPts())
            {
                Game_BattleUnit unit = _units[pt.X, pt.Y];
                if (unit != null)
                {
                    int drawingX = START_X + (pt.X * 40);
                    int drawingY = START_Y + (pt.Y * 40);
                    drawingX += unit.MovePosition().X;
                    unit.Draw(drawingX, drawingY);
                }
                if (pt.X == 4 || pt.X == 21)
                {
                    Global.Player.Wall.Draw(START_X + 4 * 40, START_Y + pt.Y * 40);
                    Global.Enemy.Wall.Draw(START_X + 21 * 40, START_Y + pt.Y * 40);
                }
            }
        }
        private void DrawEffects()
        {
            foreach (Effect_Missile arrow in _effectMissiles) arrow.UpdateDraw();
            foreach (Effect effect in _effects) effect.UpdateDraw();
        }
        //#======================================================================================
        //#----------------------------------------------------------
        //# * Process Morale Boost
        //#----------------------------------------------------------
        private void ProcessAura(Point2DEx srcPt)
        {
            Game_BattleUnit unit = _units[srcPt.X, srcPt.Y];
            foreach (Game_BattleUnit adjacentUnit in GetAdjacentUnits(srcPt))
            {
                if (!adjacentUnit.IsDead() && adjacentUnit.IsAlly(unit))
                {
                    adjacentUnit.AuraMoraleBoost += unit.MoraleBoostAmount();
                    adjacentUnit.AuraSpot += unit.SpotAmount();
                    if (unit.IsReadyToCaseHeal()) adjacentUnit.HP += unit.HealAmount();
                }
            }
        }
        //#======================================================================================
        //#----------------------------------------------------------
        //# * Process Jump (knight)
        //#----------------------------------------------------------
        private void ProcessJump(Point2DEx srcPt)
        {
            Game_BattleUnit jumpingUnit = _units[srcPt.X, srcPt.Y];
            if (jumpingUnit == null) return;
            // check if unit is blocking
            Point2DEx movePt = srcPt.Add(jumpingUnit.MoveVector());
            Game_BattleUnit moveUnit = _units[movePt.X, movePt.Y];
            if (moveUnit == null || moveUnit.HasBlockSkill()) return;
            Point2DEx destPt = srcPt.Add(jumpingUnit.JumpVector());
            Game_BattleUnit destUnit = _units[destPt.X, destPt.Y];
            if (!jumpingUnit.HasJumpSkill()) return;
            if (!jumpingUnit.IsReadyToStartAct(Game_UnitAction.Jump)) return;
            if (destUnit != null && !destUnit.IsDead()) return;
            if (IsOnOpponentZone(jumpingUnit, destPt)) return;
            JumpUnit(srcPt);
        }
        //#----------------------------------------------------------
        //# * Process Movement (of units)
        //#----------------------------------------------------------
        private void ProcessMovement(Point2DEx srcPt)
        {
            Game_BattleUnit movingUnit = _units[srcPt.X, srcPt.Y];
            if (movingUnit == null) return;
            List<Point2DEx> checkedPts = new List<Point2DEx>(); // holds collection of points that have been checked for the following recursive function
            if (IsLegalMove(movingUnit, srcPt, ref checkedPts)) // check if unit has reached end on battle zone
            {
                // draw units front to back or back to front depending on alliance, so units moving don't replace units in front
                checkedPts = (movingUnit.IsPlayerOwned ? checkedPts.OrderBy(pt => pt.X).Reverse().ToList<Point2DEx>() : checkedPts.OrderBy(pt => pt.X).ToList<Point2DEx>());
                foreach (Point2DEx pt in checkedPts) MoveUnit(pt);
            }
        }
        //#----------------------------------------------------------
        //# * Is Legal Move
        //#----------------------------------------------------------
        private bool IsLegalMove(Game_BattleUnit unit, Point2DEx srcPt, ref List<Point2DEx> checkedPts)
        {
            checkedPts.Add(srcPt);
            Point2DEx destPt = srcPt.Add(unit.MoveVector());
            if (!unit.IsReadyToStartAct(Game_UnitAction.Move)) return false; // check if unit is ready to move
            if (IsOnOpponentZone(unit, destPt)) return false; // if destination moves into reinforcement zone, return
            // if occupied, check conditions when move is still possible (dead unit or grouped unit that would move together)
            Game_BattleUnit destUnit = _units[destPt.X, destPt.Y];
            if (destUnit != null)
            {
                if (destUnit.IsAlly(unit))
                {
                    // if moving pt is grouped unit
                    if (!(unit.IsPlayerOwned ? unit.RightConnected : unit.LeftConnected)) return false;
                }
                else
                {
                    // units can move over dead enemy units
                    if (!destUnit.IsDead()) return false;
                }
            }
            // check group's ability to move
            List<Point2DEx> AdjPts = GetAdjacentPts(srcPt.X, srcPt.Y);
            for (int i = 0; i < 4; i++)
            {
                Point2DEx direction = AdjPts[i];
                if (!checkedPts.Contains(direction))
                {
                    if (unit.Connections[i] && !IsLegalMove(_units[direction.X, direction.Y], direction, ref checkedPts)) return false;
                }
            }
            return true;
        }
        //#----------------------------------------------------------
        //# * Move Unit
        //#----------------------------------------------------------
        private void MoveUnit(Point2DEx srcPt)
        {
            Game_BattleUnit unit = _units[srcPt.X, srcPt.Y];
            Point2DEx destPt = srcPt.Add(unit.MoveVector());
            unit.StartAct(Game_UnitAction.Move);
            _units[destPt.X, destPt.Y] = unit;
            _units[srcPt.X, srcPt.Y] = null;
        }
        //#----------------------------------------------------------
        //# * Jump Unit
        //#----------------------------------------------------------
        private void JumpUnit(Point2DEx srcPt)
        {
            PlaySound("jump");
            Game_BattleUnit unit = _units[srcPt.X, srcPt.Y];
            Point2DEx destPt = srcPt.Add(unit.JumpVector());
            unit.StartAct(Game_UnitAction.Jump);
            _units[destPt.X, destPt.Y] = unit;
            _units[srcPt.X, srcPt.Y] = null;
        }
        //#======================================================================================
        //#----------------------------------------------------------
        //# * Process Projectile Skill
        //#----------------------------------------------------------
        private void ProcessProjectileSkill(Point2DEx srcPt)
        {
            Game_BattleUnit actingUnit = _units[srcPt.X, srcPt.Y];
            if (!actingUnit.HasProjectileSkill()) return;
            foreach (Point2DEx projectileVector in actingUnit.MissileVectors())
            {
                Point2DEx targetPt = srcPt.Add(projectileVector);
                Game_Unit targetUnit = GetTarget(targetPt);
                if (!actingUnit.IsLegalAttackTarget(targetUnit)) continue;
                if (actingUnit.IsReadyToAct(Game_UnitAction.Projectile))
                {
                    PlaySound(actingUnit.SoundLaunch);
                    _effectMissiles.Add(new Effect_Projectile(actingUnit, srcPt, new Point2DEx(START_X + srcPt.X * 40, START_Y + srcPt.Y * 40), projectileVector.X));
                    break;
                }
                else if (actingUnit.IsReadyToStartAct(Game_UnitAction.Projectile))
                {
                    actingUnit.StartAct(Game_UnitAction.Projectile);
                    break;
                }
            }
        }
        //#----------------------------------------------------------
        //# * Process Magic Skill
        //#----------------------------------------------------------
        private void ProcessMagicSkill(Point2DEx srcPt)
        {
            Game_BattleUnit actingUnit = _units[srcPt.X, srcPt.Y];
            if (!actingUnit.HasMagicSkill()) return;
            if (actingUnit.IsReadyToAct(Game_UnitAction.Magic))
            {
                PlaySound(actingUnit.SoundLaunch);
                _effectMissiles.Add(new Effect_Magic(actingUnit, srcPt, new Point2DEx(START_X + srcPt.X * 40, START_Y + srcPt.Y * 40)));
            }
            else if (actingUnit.IsReadyToStartAct(Game_UnitAction.Magic))
            {
                foreach (Point2DEx magicVector in actingUnit.MagicVectors())
                {
                    Point2DEx targetPt = srcPt.Add(magicVector);
                    Game_Unit targetUnit = GetTarget(targetPt);
                    if (actingUnit.IsLegalAttackTarget(targetUnit))
                    {
                        actingUnit.StartAct(Game_UnitAction.Magic);
                        break;
                    }
                }
            }
        }
        //#----------------------------------------------------------
        //# * Process Attack (of units)
        //#----------------------------------------------------------
        private void ProcessAttack(Point2DEx srcPt)
        {
            Game_BattleUnit actingUnit = _units[(int)srcPt.X, (int)srcPt.Y];
            if (actingUnit.IsReadyToAct(Game_UnitAction.Attack))
            {
                Point2DEx targetPt = srcPt.Add(actingUnit.AttackVector());
                DamageHandling(actingUnit.GetDamageAgent(), targetPt);
            }
            else if (actingUnit.IsReadyToStartAct(Game_UnitAction.Attack))
            {
                Point2DEx targetPt = srcPt.Add(actingUnit.AttackVector());
                Game_Unit targetUnit = GetTarget(targetPt);
                if (actingUnit.IsLegalAttackTarget(targetUnit))
                {
                    PlaySound(actingUnit.SoundLaunch);
                    actingUnit.StartAct(Game_UnitAction.Attack);
                }
            }
        }
        //#----------------------------------------------------------
        //# * Get Target
        //#----------------------------------------------------------
        private Game_Unit GetTarget(Point2DEx pt)
        {
            if (pt.X == 4) return Global.Player.Wall;
            else if (pt.X == 21) return Global.Enemy.Wall;
            else return _units[pt.X, pt.Y];
        }
        //#----------------------------------------------------------
        //# * Damage Handling
        //#----------------------------------------------------------
        private void DamageHandling(Game_DamageAgent damageAgent, Point2DEx pt)
        {
            Game_Unit targetUnit = GetTarget(pt);
            if (damageAgent.IsLegalAttackTarget(targetUnit))
            {
                PlaySound(damageAgent.SoundHit);
                int dealtDamage = targetUnit.ExecuteDamage(damageAgent);
                _effects.Add(new Effect_Text(dealtDamage.ToString(), Color.White, START_X + pt.X * 40, START_Y + pt.Y * 40 + 20, 30));
                if (targetUnit.IsDead() && targetUnit is Game_BattleUnit)
                {
                    BreakConnections(pt); // break connections of unit
                    if (targetUnit.IsPlayerOwned) UnitsLost++;
                }
            }
        }
        //#----------------------------------------------------------
        //# * Break Connections
        //#----------------------------------------------------------
        private void BreakConnections(Point2DEx pt)
        {
            Game_BattleUnit unit = _units[pt.X, pt.Y];
            List<Point2DEx> AdjPts = GetAdjacentPts(pt.X, pt.Y);
            // iterate through connections with the unit and break them
            for (int i = 0; i < 4; i++)
            {
                // if unit has a connection, break it and break the opposite one for connecting unit
                if (unit.Connections[i])
                {
                    int toBreakId = 0;
                    switch (i)
                    {
                        case 0: toBreakId = 1; break;
                        case 1: toBreakId = 0; break;
                        case 2: toBreakId = 3; break;
                        case 3: toBreakId = 2; break;
                    }
                    _units[AdjPts[i].X, AdjPts[i].Y].Connections[toBreakId] = false;
                    unit.Connections[i] = false;
                }
            }
        }
        //#======================================================================================
        //#----------------------------------------------------------
        //# * Is In The Battlefield
        //#----------------------------------------------------------
        private bool IsValidPt(Point2DEx pt)
        {
            if (pt.X < 0 || pt.X > 25) return false;
            if (pt.Y < 0 || pt.Y > 6) return false;
            return true;
        }
        //#----------------------------------------------------------
        //# * Is On Battle Zone (the field where fighting takes place)
        //#----------------------------------------------------------
        private bool IsOnBattleZone(Point2DEx pt)
        {
            if (pt.X < REINFORCEMENT_WIDTH) return false;
            if (pt.Y < 0) return false;
            if (pt.X > REINFORCEMENT_WIDTH + BATTLEZONE_WIDTH - 1) return false;
            if (pt.Y >= BATTLEFIELD_GRID_HEIGHT) return false;
            return true;
        }
        //#----------------------------------------------------------
        //# * Is On Enemy Zone (where player reinforcements are sent in)
        //#----------------------------------------------------------
        private bool IsOnPlayerZone(Point2DEx pt)
        {
            if (pt.X > REINFORCEMENT_WIDTH - 1) return false;
            if (pt.Y < 0) return false;
            if (pt.Y >= BATTLEFIELD_GRID_HEIGHT) return false;
            return true;
        }
        //#----------------------------------------------------------
        //# * Is On Enemy Zone (where enemy reinforcements are sent in)
        //#----------------------------------------------------------
        private bool IsOnEnemyZone(Point2DEx pt)
        {
            if (pt.X < ENEMY_REINFORCEMENT_START_X) return false;
            if (pt.Y < 0) return false;
            if (pt.Y >= BATTLEFIELD_GRID_HEIGHT) return false;
            return true;
        }
        //#----------------------------------------------------------
        //# * Is On Opponent's Zone (opponent's side, dependant on unit alliance)
        //#----------------------------------------------------------
        private bool IsOnOpponentZone(Game_Unit unit, Point2DEx pt)
        {
            return (unit.IsPlayerOwned ? IsOnEnemyZone(pt) : IsOnPlayerZone(pt));
        }
        //#======================================================================================
        //#----------------------------------------------------------
        //# * Add Reinforcements
        //#----------------------------------------------------------
        public void AddReinforcements(Game_Group group, int row)
        {
            // iterate through units in group and add each one
            foreach (Point2DEx pt in group.GetUnitPts())
            {
                if (group.UnitIDs[pt.X, pt.Y] == -1) continue; // if grid doesn't contain a unit, next
                int offsetX = (group.IsPlayerOwned ? REINFORCEMENT_WIDTH - 1 - group.RightMostUnit + pt.X : ENEMY_REINFORCEMENT_START_X - group.LeftMostUnit + pt.X);
                int offsetY = row + pt.Y;
                _units[offsetX, offsetY] = group.CreateBattleUnit(pt.X, pt.Y);
            }
        }
        //#----------------------------------------------------------
        //# * Can Add Reinforcements
        //#----------------------------------------------------------
        public bool CanAddReinforcements(Game_Group group, int row)
        {
            foreach (int gridY in group.RowsWithUnits)
            {
                if (IsStartingRowOccupied(row + gridY, group.IsPlayerOwned)) return false;
            }
            return true;
        }
        //#----------------------------------------------------------
        //# * Is Starting Row Occupied
        //#----------------------------------------------------------
        private bool IsStartingRowOccupied(int row, bool playerSide)
        {
            int startingGridX = (playerSide ? 0 : ENEMY_REINFORCEMENT_START_X);
            // check a specified row for existing units
            for (int gridX = startingGridX; gridX < startingGridX + REINFORCEMENT_WIDTH; gridX++)
            {
                if (_units[gridX, row] != null) return true;
            }
            return false;
        }
        //#======================================================================================
        //#----------------------------------------------------------
        //# * Get Unit Pts
        //#----------------------------------------------------------
        private IEnumerable<Point2DEx> GetUnitPts()
        {
            for (int y = 0; y < BATTLEFIELD_GRID_HEIGHT; y++)
            {
                for (int x = 0; x < BATTLEFIELD_GRID_WIDTH; x++)
                {
                    yield return new Point2DEx(x, y);
                }
            }
        }
        //#----------------------------------------------------------
        //# * Get Adjacent Points
        //#----------------------------------------------------------
        private List<Point2DEx> GetAdjacentPts(int x, int y)
        {
            List<Point2DEx> list = new List<Point2DEx>();
            list.Add(new Point2DEx(x, y - 1));
            list.Add(new Point2DEx(x, y + 1));
            list.Add(new Point2DEx(x - 1, y));
            list.Add(new Point2DEx(x + 1, y));
            return list;
        }
        //#----------------------------------------------------------
        //# * Get Adjacent Units
        //#----------------------------------------------------------
        private List<Game_BattleUnit> GetAdjacentUnits(Point2DEx pt)
        {
            List<Game_BattleUnit> list = new List<Game_BattleUnit>();
            foreach (Point2DEx adjPt in GetAdjacentPts(pt.X, pt.Y))
            {
                if (IsValidPt(adjPt) && _units[adjPt.X, adjPt.Y] != null) list.Add(_units[adjPt.X, adjPt.Y]);
            }
            return list;
        }
        //#----------------------------------------------------------
        //# * Get Front Most Unit Pt
        //#----------------------------------------------------------
        private int GetFrontMostUnitX(int row, bool playerSide)
        {
            int battleZoneRightX = REINFORCEMENT_WIDTH + BATTLEZONE_WIDTH - 1;
            if (playerSide)
            {
                for (int x = battleZoneRightX; x >= 0; x--)
                {
                    if (_units[x, row] == null) continue;
                    if (_units[x, row].IsPlayerOwned) return x;
                }
                return 0;
            }
            else
            {
                for (int x = REINFORCEMENT_WIDTH; x < BATTLEFIELD_GRID_WIDTH; x++)
                {
                    if (_units[x, row] == null) continue;
                    if (!_units[x, row].IsPlayerOwned) return x;
                }
                return BATTLEFIELD_GRID_WIDTH - 1;
            }
        }
        //#----------------------------------------------------------
        //# * Has Player Won
        //#----------------------------------------------------------
        public bool HasPlayerWon()
        {
            return Global.Enemy.Wall.IsDead();
        }
        //#----------------------------------------------------------
        //# * Has Enemy Won
        //#----------------------------------------------------------
        public bool HasEnemyWon()
        {
            return Global.Player.Wall.IsDead();
        }
        //#----------------------------------------------------------
        //# * Play Sound
        //#----------------------------------------------------------
        private void PlaySound(string name)
        {
            if (!_playedSounds.Contains(name) && name != "null")
            {
                Resource.PlaySound(name);
                _playedSounds.Add(name);
            }
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI;

namespace AIRobot
{
    public class X2_JobDriver_GoToCellAndWait : JobDriver
    {
        public int waitTicks = (int)(GenDate.TicksPerHour * 0.55f);

        public X2_JobDriver_GoToCellAndWait() {}

        protected override IEnumerable<Toil> MakeNewToils()
        {
            // Go next to target
            //yield return GotoThing(targetCell, PathEndMode.ClosestTouch);
            //// Go directly to target
            yield return GotoThing(TargetA.Cell, Map, PathEndMode.OnCell)
                                                    .FailOnDespawnedOrNull(TargetIndex.A);

            yield return Toil_Wait(waitTicks);
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.targetA, job, 1, 0, null, errorOnFailed);
        }

        public Toil Toil_Wait(int ticks)
        {
            Toil toil = new Toil {
                defaultDuration = 32,
                defaultCompleteMode = ToilCompleteMode.Delay
            };

            toil.AddFinishAction(delegate
            {
                Pawn pawn = this.pawn;
                if (!pawn.Drafted)
                {
                    pawn.jobs.jobQueue.EnqueueFirst(new Job(JobDefOf.Wait, ticks));
                }
            });

            return toil;
        }

        public Toil GotoThing(IntVec3 cell, Map map, PathEndMode PathEndMode)
        {
            Toil toil = new Toil();
            LocalTargetInfo target = new LocalTargetInfo(cell);
            toil.initAction = delegate
            {
                Pawn actor = toil.actor;
                actor.pather.StartPath(target, PathEndMode);
            };
            toil.defaultCompleteMode = ToilCompleteMode.PatherArrival;
            //toil.AddFinishAction(new Action(DoSleep));
            return toil;
        }

    }
}

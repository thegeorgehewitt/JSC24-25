using System;
using System.Collections.Generic;

using UnityEngine;

using Custom.Controller;

namespace Custom.Interactable.Character.Comparer
{
    public class CompareCharacterMotor2D : IComparer<CharacterMotor2D>
    {
        [Flags]
        public enum CompareFlag
        {
            Visibility = 0x01,
            Distance = 0x02,
            Both = Visibility | Distance
        }

        private Vector3 characterPos;
        private CompareFlag compareFlag = CompareFlag.Both;


        /// <param name="_firePoint"> Position of the character calling this compare. </param>
        /// <param name="_compareFlag">  Which properties should be used to compare? </param>
        public CompareCharacterMotor2D(
            Vector3 _firePoint,
            CompareFlag _compareFlag = CompareFlag.Both)
        {
            characterPos = _firePoint;
            compareFlag = _compareFlag;
        }


        public int Compare(CharacterMotor2D _A, CharacterMotor2D _B)
        {
            // Compare visibility and take the most visible motor.
            if ((compareFlag & CompareFlag.Visibility) == CompareFlag.Visibility)
            {
                if (_A.Visibility > _B.Visibility) return 1;
                else if (_B.Visibility > _A.Visibility) return -1;
            }

            // Compare distance if visibility of both motors are the same.
            if ((compareFlag & CompareFlag.Distance) == CompareFlag.Distance)
            {
                float disA = Vector3.Distance(_A.transform.position, characterPos);
                float disB = Vector3.Distance(_B.transform.position, characterPos);

                if (disB > disA) return 1;
                else if (disB < disA) return -1;
            }

            return 0;
        }
    }
}

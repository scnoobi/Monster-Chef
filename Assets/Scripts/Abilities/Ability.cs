using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class Ability : MonoBehaviour
{
   public int id;
   public abstract void castAbility();
}

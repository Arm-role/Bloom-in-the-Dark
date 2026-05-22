using System;
using System.Collections.Generic;
using UnityEngine;

// 1 รายการแลกของ — ใส่ inputs ครบ แลกได้ output (barter, แลกได้ไม่จำกัดครั้ง)
[Serializable]
public class TradeOffer
{
  [SerializeField] private List<Ingredient> inputs;
  [SerializeField] private Ingredient output;

  public IReadOnlyList<Ingredient> Inputs => inputs;
  public Ingredient Output => output;
}

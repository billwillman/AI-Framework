using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAbilityPlay : MonoBehaviour
{

    private AnimancerAbilityLinker m_Linker = null;

    public AnimancerAbilityLinker Linker {
        get {
            if (m_Linker == null)
                m_Linker = GetComponent<AnimancerAbilityLinker>();
            return m_Linker;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W)) {
            var linker = this.Linker;
            if (linker != null) {
                linker.TryStartAbility("TestAbility");
            }
        } else if (Input.GetKeyDown(KeyCode.S)) {
            var linker = this.Linker;
            if (linker != null) {
                linker.TryStartAbility("TestAbility1");
            }
        }
    }
}

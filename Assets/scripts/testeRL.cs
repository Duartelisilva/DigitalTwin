using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script to set Automatico's public variables
public class testeRL : MonoBehaviour
{  
    public GameObject carro;
    public int movimento = 0;
    public float angulo = 0;
    
    void FixedUpdate()
    {
        // Assuming automaticoScript is assigned in the Unity Editor
        movimentoautomatico automaticoScript = carro.GetComponent<movimentoautomatico>();


        automaticoScript.movementInput = movimento;
        automaticoScript.wantedangle = angulo;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControleDoJogador : MonoBehaviour
{

    // Vari�veis �teis para o script do jogador
    CharacterController cc; // Controlador do personagem
    float velocidade = 5f;
    float gravidade = 10f;

    void Start()
    {
        // Vamos iniciar o controlador aqui no Start
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {

        // Inputs detectados pressionado pelo usu�rio
        float inputHorizontal = Input.GetAxis("Horizontal");
        float inputVertical = Input.GetAxisRaw("Vertical");


        Debug.Log("Vertical �: " + inputVertical);

        // Converter os inputs em dire��o no jogo
        float direcao_z = inputVertical * Time.deltaTime * velocidade;
        float direcao_x = inputHorizontal * Time.deltaTime * velocidade;
        float direcao_y = -gravidade * Time.deltaTime;

        // Move o componente do Character Controller
        Vector3 direcao_movimento = new Vector3(direcao_x, direcao_y, direcao_z);
        cc.Move( direcao_movimento );

    }
}

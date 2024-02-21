using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControleDoJogador : MonoBehaviour
{

    // Variáveis úteis para o script do jogador
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

        // Inputs detectados pressionado pelo usuário
        float inputHorizontal = Input.GetAxis("Horizontal");
        float inputVertical = Input.GetAxisRaw("Vertical");


        Debug.Log("Vertical é: " + inputVertical);

        // Converter os inputs em direção no jogo
        float direcao_z = inputVertical * Time.deltaTime * velocidade;
        float direcao_x = inputHorizontal * Time.deltaTime * velocidade;
        float direcao_y = -gravidade * Time.deltaTime;

        // Move o componente do Character Controller
        Vector3 direcao_movimento = new Vector3(direcao_x, direcao_y, direcao_z);
        cc.Move( direcao_movimento );

    }
}

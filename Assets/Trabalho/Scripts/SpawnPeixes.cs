using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPeixes : MonoBehaviour
{
    [Header("Transform que representa o 'centro' ou referência do Spawn")]
    [SerializeField] private Transform spawnArea;

    [Header("Limites no eixo X para o spawn")]
    [SerializeField] private float xStart = -2f;
    [SerializeField] private float xEnd = 2f;

    [Header("Limites no eixo Y para o spawn")]
    [SerializeField] private float yStart = -2f;
    [SerializeField] private float yEnd = 2f;

    [Header("Lista de Prefabs a serem spawnados")]
    [SerializeField] private List<GameObject> prefabs;

    [Header("Quantos objetos serão criados inicialmente?")]
    [SerializeField] private int quantidade = 1;

    // Quantidade de objetos ativos ou instanciados atualmente
    [SerializeField] private int quantidade_On = 0;

    [Header("Delay to Spawn in the begining")]
    [SerializeField] private float minDelay = 0.5f;    // Minimum time delay
    [SerializeField] private float maxDelay = 2f;      // Maximum time delay

    private void Start()
    {
        StartCoroutine(SpawnFishCoroutine());
    }

    private void Update()
    {
        // Quando chegar a zero (por exemplo, se forem destruídos), gera novamente
        if (quantidade_On < quantidade)
        {
            SpawnObjetos();
        }
    }

    // Método para spawnar os objetos
    public void SpawnObjetos()
    {
        for (int i = 0; i < quantidade; i++)
        {
            // Gera coordenadas aleatórias dentro dos limites
            float randomX = Random.Range(xStart, xEnd);
            float randomY = Random.Range(yStart, yEnd);

            // Monta a posição final (usando o Z do 'spawnArea', se quiser manter)
            Vector3 spawnPos = new Vector3(
                randomX,
                randomY,
                spawnArea.position.z
            );

            // Escolhe aleatoriamente um prefab da lista
            int indexAleatorio = Random.Range(0, prefabs.Count);
            GameObject prefabEscolhido = prefabs[indexAleatorio];

            // Instancia o prefab
            Instantiate(prefabEscolhido, spawnPos, Quaternion.Euler(0, -110.552f, 0));

            // Incrementa o contador
            quantidade_On++;
        }
    }

    private IEnumerator SpawnFishCoroutine()
    {
        for (int i = 0; i < quantidade; i++)
        {
            // Generate random position within the area
            float randomX = Random.Range(xStart, xEnd);
            float randomY = Random.Range(yStart, yEnd);
            Vector3 spawnPos = new Vector3(randomX, randomY, spawnArea.position.z);

            // Select a random prefab
            int indexAleatorio = Random.Range(0, prefabs.Count);
            GameObject prefabEscolhido = prefabs[indexAleatorio];

            // Instantiate the prefab
            Instantiate(prefabEscolhido, spawnPos, Quaternion.Euler(0, -110.552f, 0));

            // Increment counter
            quantidade_On++;

            // Wait for a random delay before spawning the next fish
            float randomDelay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(randomDelay);
        }
    }

    private void OnDrawGizmos()
    {
        if (spawnArea == null) return;

        Gizmos.color = Color.green;

        // Calcula os cantos do retângulo
        Vector3 bottomLeft = new Vector3(xStart, yStart, spawnArea.position.z);
        Vector3 bottomRight = new Vector3(xEnd, yStart, spawnArea.position.z);
        Vector3 topLeft = new Vector3(xStart, yEnd, spawnArea.position.z);
        Vector3 topRight = new Vector3(xEnd, yEnd, spawnArea.position.z);

        // Desenha as linhas do retângulo
        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{

    // Titik-titik yang diatur sebagai node untuk musuh berpatroli di antaranya
    [SerializeField]
    private Transform point1;
    [SerializeField]
    private Transform point2;

    // Variabel yang di-serialize untuk jarak deteksi dan laju tembakan
    [SerializeField]
    private float detectionRange;
    [SerializeField]
    private float firingRange;

    // Prefab material untuk status siaga (misalnya berwarna merah)
    [SerializeField]
    private Material alertColor;

    private Material normalColor;

    private MeshRenderer thisRenderer;

    private Vector3 targetNode;
    private Vector3 destinationNode;

    private float destinationNumber;
    private float distance;

    // State digunakan untuk pernyataan switch dan mesin state
    private int state;

    // Boolean yang memberi tahu musuh untuk mengubah warna
    private bool isAlert;

    // GameObject untuk merujuk pemain menggunakan tagnya
    private GameObject player;

    private NavMeshAgent thisAgent;

    void Start()
    {
        // Dapatkan komponen agent untuk navigasi
        thisAgent = GetComponent<NavMeshAgent>();

        // Temukan pemain menggunakan tagnya
        player = GameObject.FindGameObjectWithTag("Player");

        state = 0;
        destinationNumber = 2;

        // Atur renderer dan kemudian atur warna normal ke warna awal musuh
        thisRenderer = GetComponent<MeshRenderer>();
        normalColor = thisRenderer.material;
    }

    void Update()
    {
        // Ini adalah mesin state, digunakan untuk mengubah perilaku AI
        switch (state)
        {
            case 0:
                // Berpatroli
                {
                    Patrolling();

                    isAlert = false;

                    break;
                }
            case 1:
                // Menyerang
                {
                    Attacking(false);

                    isAlert = true;

                    break;
                }
            case 2:
                // Menembak
                {
                    Attacking(true);

                    isAlert = true;

                    break;
                }
            case 3:
                // Mengejar
                {
                    DetectPlayer();

                    break;
                }
        }
        // Terus-menerus mengatur destinasi ke node destinasi
        thisAgent.SetDestination(destinationNode);
        // Terus memeriksa apakah musuh harus muncul sebagai status siaga (ubah warna)
        AlertActive(isAlert);

        // Jarak dari pemain
        distance = Vector3.Distance(transform.position, player.transform.position);

        // Periksa apakah pemain berada dalam jangkauan untuk memulai serangan
        if (distance <= detectionRange)
        {
            Vector3 direction = player.transform.position - transform.position;

            RaycastHit hit;

            // Gunakan raycast untuk memeriksa garis pandang ke pemain
            if (Physics.Raycast(transform.position, direction, out hit, Mathf.Infinity))
            {
                // Tag objek awal diperiksa untuk memastikan bahwa tidak ada yang menghalangi
                // musuh dari pemain, seperti tembok atau lanskap lain
                if (hit.collider.tag == "Player")
                {
                    // Periksa jarak untuk jangkauan tembakan
                    if (distance <= firingRange)
                    {
                        state = 2;
                    }
                    else
                    {
                        state = 1;
                    }

                    Debug.DrawRay(transform.position, direction, Color.green);
                }
                else
                {
                    // Ini untuk jika sinar mengenai rintangan: kita ingin AI tetap berjalan seperti biasa
                    // dan tidak mulai menyerang melalui tembok atau tampaknya bisa melihat melalui tembok!

                    Debug.DrawRay(transform.position, direction, Color.red);

                    if (state == 0)
                    {
                        return;
                    }
                    else
                    {
                        state = 3;
                    }
                }
            }
        }
    }

    void AlertActive(bool alertState)
    {
        // Representasi visual dari status siaga, seperti perubahan warna merah

        if (alertState)
        {
            thisRenderer.material = alertColor;
        }
        else
        {
            thisRenderer.material = normalColor;
        }
    }

    void Patrolling()
    {
        // Ini mengatur titik-titik sebagai destinasi untuk musuh melakukan patroli di antaranya
        // dan kemudian berpindah ke titik kedua untuk patroli sederhana yang melompat antara dua titik

        if (destinationNumber == 1)
        {
            destinationNode = point1.position;

            distance = Vector3.Distance(transform.position, destinationNode);

            if (distance <= 1f)
            {
                destinationNumber = 2;
            }
        }

        if (destinationNumber == 2)
        {
            destinationNode = point2.position;

            distance = Vector3.Distance(transform.position, destinationNode);

            if (distance <= 1f)
            {
                destinationNumber = 1;
            }
        }
    }

    void Attacking(bool isFiring)
    {
        // Mengatur destinasi ke pemain: musuh akan mengikuti pemain 
        destinationNode = player.transform.position;
    }

    void FireAtTarget(GameObject target)
    {
        // Atur posisi yang akan diambil peluru sebagai targetnya
        targetNode = target.transform.position;

        // Mengatur target di dalam skrip peluru sebagai posisi saat ini pemain
    }

    void DetectPlayer()
    {
        // Fungsi ini hanya membuat musuh terus bergerak ke titik terakhir yang diketahui pemain
        // untuk membuatnya tampak seolah-olah sedang mengejar pemain sampai pemain bersembunyi

        distance = Vector3.Distance(transform.position, destinationNode);

        if (distance <= 1f)
        {
            state = 0;
        }
    }
}

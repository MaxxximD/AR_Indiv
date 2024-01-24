using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThrowShell : MonoBehaviour
{
    private ProgrammManager ProgrammManagerScript;
    private TrajectoryRenderer TrajectoryRenderScript;

    [SerializeField] private GameObject ShellPrefab;
    private GameObject ShellObject;
    private Rigidbody ShellRigidBody;
    private Vector3 speed;

    private GameObject fieldobject;
    private InputField filed;
    private string ForceString;
    private int force;
    private Rigidbody CollisionRigidBody;

    public AudioClip ThrowSound;
    private AudioSource CatapultAudio;

    // Start is called before the first frame update
    void Start()
    {
        ProgrammManagerScript = FindObjectOfType<ProgrammManager>();
        TrajectoryRenderScript = FindObjectOfType<TrajectoryRenderer>();
        fieldobject = GameObject.Find("InputField");
        filed = fieldobject.GetComponent<InputField>();
    }

    // Update is called once per frame
    void Update()
    {
        ForceString = filed.text;
        force = Int32.Parse(ForceString);     
        //сложение двух векторов
        speed = transform.forward * 2 + transform.up * force;
        //рассчет траектории полета снаряда 
        TrajectoryRenderScript.ShowTrajectory(transform.position + new Vector3(0, 0.25f, 0), speed);
    }

    //анимация выстрела
    private void OnCollisionEnter(Collision collision)
    {
        //создание ядра 
        ShellObject = Instantiate(ShellPrefab, transform.position + new Vector3(0, 0.25f, -0.05f), ShellPrefab.transform.rotation);
        ShellRigidBody = ShellObject.GetComponent<Rigidbody>();
        //запускаем снаряд
        ShellRigidBody.AddForce(speed, ForceMode.Impulse);
       
        //объет, с которым мы пересеклись(рычаг)
        CollisionRigidBody = collision.rigidbody;
        //отбрасываем его в исходное положение
        CollisionRigidBody.AddForce(CollisionRigidBody.transform.up * (-1), ForceMode.Impulse); 

        //перезарядка(появление очередного снаряда)
        ProgrammManagerScript.Recharging = true;

        //CatapultAudio = GetComponent<AudioSource>();
        //CatapultAudio.PlayOneShot(ThrowSound, 1.0f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sym4D;
using Sym = Sym4D.Sym4DEmulator;

public class SensorManager : MonoBehaviour
{

    public UnityEvent aaa;

    // 포트번호 (COM 포트)
    private int xPort; // 의자 컨트롤 포트번호
    private int wPort; // 팬 컨트롤 포트번호
    private bool isWindy = false; // 팬 동작 여부

    private WaitForSeconds ws = new WaitForSeconds(0.1f);

    // Joystick 변수
    private float prevJoyX, prevJoyY;
    private float currJoyX, currJoyY;

    IEnumerator Start()
    {
        StartCoroutine(InitSym4D());

        yield return new WaitForSeconds(1.0f);

        StartCoroutine(TestDevice());
    }

    IEnumerator TestDevice()
    {
        StartCoroutine(SetMotion(-10, 0));
        yield return new WaitForSeconds(2.0f);

        StartCoroutine(SetMotion(10, 0));
        yield return new WaitForSeconds(2.0f);

        StartCoroutine(SetMotion(0, -10));
        yield return new WaitForSeconds(2.0f);

        StartCoroutine(SetMotion(0, 10));
        yield return new WaitForSeconds(2.0f);

        StartCoroutine(SetMotion(10, 10));
        yield return new WaitForSeconds(2.0f);

        StartCoroutine(SetMotion(-10, -10));
        yield return new WaitForSeconds(2.0f);

        StartCoroutine(SetWind(100));
        yield return new WaitForSeconds(2.0f);

        StartCoroutine(SetWind(0));
        yield return new WaitForSeconds(2.0f);
    }

    // Sym4D 초기화
    IEnumerator InitSym4D()
    {
        // 포트번호 추출
        xPort = Sym.Sym4D_X_Find();
        yield return ws;

        wPort = Sym.Sym4D_W_Find();
        yield return ws;

        // 포트 오픈 및 설정
        Sym.Sym4D_X_StartContents(xPort);
        yield return ws;

        // 의자의 각도 설정 (-10도 ~ +10도)
        Sym.Sym4D_X_SetConfig(100, 100);
        yield return ws;

        // 팬 포트 오픈 및 설정
        Sym.Sym4D_W_StartContents(wPort);
        yield return ws;

        // 팬의 속도 설정
        Sym.Sym4D_W_SetConfig(100);
        yield return ws;
    }

    IEnumerator SetMotion(int roll, int pitch)
    {
        // 동작시작
        Sym.Sym4D_X_StartContents(xPort);
        yield return ws;

        // 동작 명령
        Sym.Sym4D_X_SendMosionData(roll * 10, pitch * 10);
        yield return ws;
    }

    IEnumerator SetWind(int speed)
    {
        Sym.Sym4D_W_StartContents(wPort);
        yield return ws;

        isWindy = !isWindy;

        Sym.Sym4D_W_SendMosionData(isWindy ? speed : 0);
        yield return ws;
    }

    void Update()
    {
        currJoyX = Input.GetAxis("Horizontal"); // -1.0f ~ +1.0f
        currJoyY = Input.GetAxis("Vertical");

        if (currJoyX != prevJoyX || currJoyY != prevJoyY)
        {
            // Roll 
            StartCoroutine(SetMotion((int)(currJoyX * 10.0f), (int)(currJoyY * 10.0f)));
            prevJoyX = currJoyX;
            prevJoyY = currJoyY;
        }

        if (Input.GetKeyDown(KeyCode.Joystick1Button8))
        {
            StartCoroutine(SetWind(100));
        }

        if (Input.GetKeyDown(KeyCode.Joystick1Button12))
        {
            StartCoroutine(SetWind(0));
        }
    }

    void OnDestroy()
    {
        Sym.Sym4D_X_EndContents();
        Sym.Sym4D_W_EndContents();
    }



}

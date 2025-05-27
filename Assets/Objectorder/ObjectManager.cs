using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
//Target Object
public class ObjectManager : MonoBehaviour
{
    public GameObject[] Target = new GameObject[7];
    // Start is called before the first frame update
    public GameObject handObject;
    public BulletScript gun;
    public FingerGun finger;
    public string clickname;
    public static int rndInt;
    public static int Level=1;
    public static int Score=0;
    public static int life=3;
    public int Integer;
    public bool isPassed;
    private bool clickOccurred;
    public string expected;
    public List<int> randomlist = new List<int>();
    public List<AudioClip> audioClips = new List<AudioClip>();
    public float flash=1f;
    public void PlayAudio(int index)
    {
        if (index >= 0 && index < audioClips.Count)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.clip = audioClips[index];
            audioSource.Play();
        }
        else
        {
            Debug.LogError("잘못된 인덱스입니다.");
        }
    }
    public static int Sendrandom()
    {
        rndInt = Random.Range(0, 7);
        return rndInt;
    }
    void Start()
    {
        rndInt = 0;
        gun=GameObject.Find("bullet").GetComponent<BulletScript>();
        //GameObject targetObj = GameObject.Find("BulletObject");
        audioClips.Add(Resources.Load<AudioClip>("Sounds/Do"));
        audioClips.Add(Resources.Load<AudioClip>("Sounds/Re"));
        audioClips.Add(Resources.Load<AudioClip>("Sounds/Mi"));
        audioClips.Add(Resources.Load<AudioClip>("Sounds/Fa"));
        audioClips.Add(Resources.Load<AudioClip>("Sounds/Sol"));
        audioClips.Add(Resources.Load<AudioClip>("Sounds/Ra"));
        audioClips.Add(Resources.Load<AudioClip>("Sounds/Si"));

        Target[0] = GameObject.Find("Do");
        Target[1] = GameObject.Find("Re");
        Target[2] = GameObject.Find("Mi");
        Target[3] = GameObject.Find("Fa");
        Target[4] = GameObject.Find("Sol");
        Target[5] = GameObject.Find("La");
        Target[6] = GameObject.Find("Si");
        StartCoroutine(GameManager());
    }

    private IEnumerator GameManager()
    {
        while(true) {
            rndInt = Sendrandom();
            randomlist.Add(rndInt);
            for(int i=0; i<Level; i++) {
                Integer = randomlist[i];
                Debug.Log("랜덤 오브젝트:"+ Target[Integer]);
                yield return new WaitForSeconds(0.5f);
                StartCoroutine(objectsound());
                StartCoroutine(Emisisioneffect(Integer));
                yield return new WaitForSeconds(2f);    
            }
            
            for(int i=0; i<Level; i++) {      
                isPassed = false;
                Debug.Log("플래그 "+isPassed);
                yield return new WaitUntil(() => isPassed);
                Debug.Log("isPassed 정상");
                Integer = randomlist[i];
                Debug.Log("선택한 오브젝트"+ expected);
                if (Target[Integer].name == expected)
                {
                    // 찾았다!
                    Score += Level;
                    Debug.Log("정답 " + (i + 1));
                    StartCoroutine(Emisisioneffect(Integer));
                }else {
                    life  -= 1;
                    Score -= 5;
                    Debug.Log("다시하세요. 남은 횟수는 " + life + " (" + (i + 1) + ")");
                    i--;    // 같은 i 반복
                    continue;
                }
            }
            yield return new WaitForSeconds(1f);
            
            Debug.Log("레벨 업"+(Level+1));
            Level++;
        
        
        }
    }
    void Update()
    {
        isPassed= false;
        var bullets = FindObjectsOfType<BulletScript>();
        //Debug.Log($"[Debug] 총알 개수 = {bullets.Length}");
        foreach (var bs in bullets)
        {
            //Debug.Log($"  {bs.name} → isCollision = {bs.isCollision}");
            if (bs.isCollision)
                //Debug.Log($"[Finder] {bs.name} passed → {bs.ExpectedObject}");
                isPassed = bs.isCollision;
                bs.isCollision = false;
                expected = bs.ExpectedObject;
                break;
        }
        
    }
    IEnumerator objectsound()
    {
        yield return new WaitForSeconds(1f);
        for(int i=0;i<randomlist.Count;i++)
        {
            switch(randomlist[i])
            {
                case 0:
                    PlayAudio(randomlist[i]);
                    break;
                case 1:
                    PlayAudio(randomlist[i]);
                    break;
                case 2:
                    PlayAudio(randomlist[i]);
                    break;
                case 3:
                    PlayAudio(randomlist[i]);
                    break;
                case 4:
                    PlayAudio(randomlist[i]);
                    break;
                case 5:
                    PlayAudio(randomlist[i]);
                    break;
                case 6:
                    PlayAudio(randomlist[i]);
                    break; 

            }

        }
    }
    IEnumerator Emisisioneffect(int index){
        Material mat = Target[index].GetComponent<Renderer>().material;
        

        for(int i=0;i<randomlist.Count;i++)
        {
            if(i<randomlist.Count)
            {
                mat.EnableKeyword("_EMISSION");
                yield return new WaitForSeconds(flash);
                mat.DisableKeyword("_EMISSION");
            }

        }
        yield return new WaitForSeconds(1f);

    }

}

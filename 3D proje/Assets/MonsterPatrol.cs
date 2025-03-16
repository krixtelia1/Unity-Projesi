using System.Collections;
using UnityEngine;
using UnityEngine.AI;


public class MonsterPatrol : MonoBehaviour
{
    //görüş mesafesi için düşman tespiti yapılacak çemberin yarıçapı
    public float viewRadius = 10f;
    //görüş mesafesinin yarı çapı (45 derece vesaire)
    public float viewAngle = 90f;
    //oyuncu katman maskesi
    public LayerMask playerLayer;
    //engel katman maskesi
    public LayerMask obstacleMask;

    //oyuncu atamak için kullanılan değişken (karakter obje atama)
    private Transform player;
    //canavar aktif olarak kovalıyor mu
    private bool isChasing;




    //Devriye ile ilgili
    //devriye noktalarını tek tek atadığımız trnasform array
    public Transform[] patrolPoints;
    //canavar anlık olarak hangi devriye noktasında olduğunu belirten bir değişken
    private int currentPatrolIndex;
    //canavar yapay zekasını kontrol ettiğimiz navmesh bileşeni
    private NavMeshAgent agent;

    //oyuncuyu gördüğü noktada ne kadar süre bekleyecek
    public float waitTimeAtLastSeen;
    //oyuncuyu son gördüğü konum
    private Vector3 lastSeenPosition;
    //oyuncuyu son gördüğü noktada bekliyor mu
    private bool waitingAtLastSeen;


    private void Start() //oyun başında bir kez çalışır 
    {
        //scriptin atılı olduğu objeden navmeshagent adlı bileşeni alır ve atamasını yapar
        agent= GetComponent<NavMeshAgent>();
        //bütün oyun sahnesini tarar player tagini bulup atar
        player = GameObject.FindGameObjectWithTag("Player").transform; //update de kullanılmaz
        //canavara başlangıçta ilk devriye noktası atar
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);


    }

    //canavarın karakteri ilk gördüğü pozisyon ile son gördüğü pozisyon aynıdır
    private void CheckForPlayer()
    {
        //canavarın etrafında viewRadius yarıçapı oluşturur ve oyuncuyu arar
        Collider[] hits = Physics.OverlapSphere(transform.position, viewRadius, playerLayer);

        //eğer oyuncu buldıysa bu satırı çalıştırır
        foreach (var hit in hits) //hit = oyuncu
        {
            //oyuncu ve canavarın pozisyonunu temel alarak canavarın oyunvuya doğru yönünü hesaplar
            Vector3 directionToPlayer = (hit.transform.position - transform.position).normalized; //oyuncuya göre canavarın yön belirlemesi
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer); //açı hesaplama

            //eğer oyuncu görüş açısında ise burayı çalıştırr değilse pas geçer
            if (angleToPlayer < viewAngle/2)
            {
                //canavardan oyuncuya doğru bir ışın gönderir ve oyuncuya çarpıp çarpmadığını kontrol eder
                if (!Physics.Linecast(transform.position, hit.transform.position, obstacleMask)) 
                {
                    //en son görülen pozisyonu ıiığının çarptığı mevcut pozisyona tekrar atama yapar
                    lastSeenPosition= hit.transform.position;
                    //oyuncuyu kovalamaya gider
                    ChasePlayer(hit.transform);
                    //methodu bitirir
                    return;
                }
            }

        }
        //eğer oyuncuyu kovalıyorsa
        if (isChasing)
        {
            //kovalamayı bitirir
            isChasing = false;
            //son gördüğü pozisyona doğru devam eder
            StartCoroutine(GoToLastSeenPosition());
            //en yakın devriye noktasına git
        }

    }
    //canavara verilen bir emir komutudur oyuncuyu yakalamasını sağlar 
    private void ChasePlayer(Transform playerTransform) 
    {
        //canavr ajanını hedefini oyuncu olarak atar 
        agent.SetDestination(playerTransform.position);
        //canavar kovalamacada mı değişkenini true yapar
        isChasing= true;
    }
    //canavar yakalama sonrasında 
    private Transform FindClosestPatrolPoint()
    {
        Transform closestPatrolPoint= null;
        float minDistance = Mathf.Infinity;

        foreach (var point in patrolPoints)
        {
            float distance = Vector3.Distance(transform.position, point.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestPatrolPoint = point;
            }
        }
        return closestPatrolPoint;
    }

    private void GoToNearestPatrolPoint()
    {
        Transform closestPoint = FindClosestPatrolPoint();
        if (closestPoint != null)
        {
            agent.SetDestination(closestPoint.position);
        }


    }

    private void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        currentPatrolIndex = (currentPatrolIndex +1) % patrolPoints.Length;
    }



    private void Update()
    {
        CheckForPlayer();

        if (!isChasing && !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GoToNextPatrolPoint();
        }

    }

    private IEnumerator GoToLastSeenPosition()
    {
        //eğer son görülen noktada görülüyorsa methodu çalıştırma
        if (waitingAtLastSeen) yield break;

        //son görülen noktaya doğru hareket et ve son görülen noktada bekle
        agent.SetDestination(lastSeenPosition);
        waitingAtLastSeen= true;

        //hedefe ulaşmasını bekle
        while(!agent.pathPending && agent.remainingDistance > 0.05f)
        {
            yield return null;
        }

        float elapsedTime = 0f;
        agent.isStopped = true;
        while (elapsedTime < waitTimeAtLastSeen)
        {
            CheckForPlayer();
            if (isChasing)
            {
                agent.isStopped= false;
                waitingAtLastSeen= false;
                yield break;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        agent.isStopped = false; 
        
        //belirli bir zaman bekledikten sonra beklemeyi bırak ve en yakın devriye noktasına dön
        waitingAtLastSeen= false;
        GoToNearestPatrolPoint();
    }
}

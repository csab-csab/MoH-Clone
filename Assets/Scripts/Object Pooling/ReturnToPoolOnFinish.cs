using UnityEngine;

public class ReturnToPool : MonoBehaviour
{
    [SerializeField] private float lifeTimeIfNotParticleSystem = 5f;

    private ParticleSystem _particleSystem;
    private bool _isTimerMode;
    private float _currentTime;
    private bool _hasEmitted;

    private void Awake()
    {
        if (!TryGetComponent(out _particleSystem))
        {
            _isTimerMode = true;
        }
    }

    private void OnEnable()
    {
        if (_isTimerMode)
        {
            _currentTime = lifeTimeIfNotParticleSystem;
        }
        else
        {
            _hasEmitted = false;
            _particleSystem.Clear();
            _particleSystem.Play();
        }
    }

    private void Update()
    {
        if (_isTimerMode)
        {
            _currentTime -= Time.deltaTime;
            if (_currentTime <= 0f)
            {
                gameObject.SetActive(false);
            }
            return;
        }

        // Wait until particles are registered as alive before checking if they died
        if (!_hasEmitted)
        {
            if (_particleSystem.IsAlive(true))
            {
                _hasEmitted = true;
            }
            return;
        }

        // Now safe to disable when all particles finish
        if (!_particleSystem.IsAlive(true))
        {
            gameObject.SetActive(false);
        }
    }
}
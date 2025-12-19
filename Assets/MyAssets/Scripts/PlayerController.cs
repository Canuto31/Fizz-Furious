using System.Collections;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerController : MonoBehaviour, IDamageable
{
    #region === COMPONENTS ===

    public Rigidbody rb;
    [SerializeField] private MeshRenderer _meshRenderer;
    private Color _originalColor;

    #endregion

    #region === MOVEMENT ===

    private Vector2 _inputVector;
    public float speed;

    public float jumpForce;
    public Transform groundCheck;
    public float groundRadius;
    public LayerMask groundLayer;

    private bool _isGrounded;

    #endregion

    #region === RUN / STAMINA ===

    [Header("Run / Stamina")] public float walkSpeed = 5f;
    public float runSpeed = 8f;

    public float maxStamina = 100f;
    public float staminaDrainPerSecond = 30f;
    public float staminaRegenPerSecond = 20f;

    [SerializeField]private float _currentStamina;
    private bool _isRunning;

    #endregion

    #region === COMBAT CONFIG ===

    public float basicHitDamage;
    public float heavyHitDamage;

    public float knockbackBasicForce = 2f;
    public float knockbackHeavyForce = 5f;
    public float knockbackUpForce = 1.5f;

    public float heavyChargeTime = 1f;

    [SerializeField] private float deathY = -10f;
    private bool _isDead;

    #endregion

    #region === COMBAT STATE ===

    private bool _canHitEnemy;
    private IDamageable _targetInRange;

    private bool _isChargingHeavy;
    private bool _heavyHitExecuted;
    private float _heavyChargeTimer;

    #endregion

    #region === FX ===

    public ParticleSystem basicHitFX;
    public ParticleSystem heavyHitFX;

    #endregion

    #region === AUDIO ===

    [Header("Audio")] public AudioSource audioSource;
    public AudioClip basicHitsFX;
    public AudioClip heavyHitsFX;
    public AudioClip missHitsFX;
    public AudioClip heavyChargesFX;

    #endregion

    #region === FAKE ANIMATION ===

    [Header("Fake Animation")] [SerializeField]
    private FakeHitAnimation fakeHitAnimation;

    #endregion

    #region === CAMERA / HIT STOP ===

    [Header("Camera Shake")] public CameraShake cameraShake;
    public float basicShakeIntensity = 0.08f;
    public float heavyShakeIntensity = 0.15f;
    public float shakeDuration = 0.15f;

    [Header("Hit Stop")] public float hitStopDuration = 0.07f;

    #endregion

    #region === STATS ===

    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    #endregion

    #region === UNITY CALLBACKS ===

    private void Awake()
    {
        _originalColor = _meshRenderer.material.color;

        _currentStamina = maxStamina;
        
        currentHealth = maxHealth;
    }

    private void Update()
    {
        HandleHeavyCharge();
        HandleStamina();
        CheckOutOfBounds();
    }

    private void FixedUpdate()
    {
        MovePlayer();
        CheckGround();
    }

    #endregion

    #region === INPUT CALLBACKS ===

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
            _inputVector = context.ReadValue<Vector2>();
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started && _currentStamina > 0f)
            _isRunning = true;

        if (context.canceled)
            _isRunning = false;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && _isGrounded)
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    public void OnBasicHit(InputAction.CallbackContext context)
    {
        if (context.started)
            DoBasicHit();
    }

    public void OnHeavyHit(InputAction.CallbackContext context)
    {
        if (context.started)
            StartHeavyCharge();

        if (context.canceled)
            CancelHeavyCharge();
    }

    #endregion
    
    #region === HEALTH LOGIC ===
    public void Heal(float healAmount, float maxHealhReduction)
    {
        maxHealth -= maxHealhReduction;
        maxHealth = Mathf.Max(maxHealth, 10f);

        currentHealth += healAmount;
        
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }
    #endregion

    #region === MOVEMENT LOGIC ===

    private void MovePlayer()
    {
        Vector3 direction = (transform.forward * _inputVector.y) +
                            (transform.right * _inputVector.x);

        float currentSpeed = _isRunning && _currentStamina > 0f ? runSpeed : walkSpeed;

        rb.MovePosition(rb.position + direction * (currentSpeed * Time.fixedDeltaTime));
    }

    private void CheckGround()
    {
        _isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundRadius,
            groundLayer
        );
    }

    #endregion

    #region === STAMINA LOGIC ===

    private void HandleStamina()
    {
        if (_isRunning && _inputVector.magnitude > 0.1f)
        {
            _currentStamina -= staminaDrainPerSecond * Time.deltaTime;

            if (_currentStamina <= 0f)
            {
                _currentStamina = 0f;
                _isRunning = false;
            }
        }
        else
        {
            _currentStamina += staminaRegenPerSecond * Time.deltaTime;
            _currentStamina = Mathf.Clamp(_currentStamina, 0f, maxStamina);
        }
    }

    #endregion

    #region === COMBAT LOGIC ===

    private void DoBasicHit()
    {
        if (!_canHitEnemy || _targetInRange == null)
        {
            PlaySound(missHitsFX);
            return;
        }

        PlaySound(basicHitsFX);
        StartCoroutine(HitStop(hitStopDuration));
        cameraShake?.Shake(basicShakeIntensity, shakeDuration);

        _targetInRange.TakeDamage(
            basicHitDamage,
            transform,
            knockbackBasicForce
        );
    }

    private void DoHeavyHit()
    {
        _targetInRange.TakeDamage(
            heavyHitDamage,
            transform,
            knockbackHeavyForce
        );
    }

    #endregion

    #region === HEAVY ATTACK ===

    private void StartHeavyCharge()
    {
        _isChargingHeavy = true;
        _heavyHitExecuted = false;
        _heavyChargeTimer = 0f;

        PlayLoopSound(heavyChargesFX);
    }

    private void HandleHeavyCharge()
    {
        if (!_isChargingHeavy || _heavyHitExecuted)
            return;

        _heavyChargeTimer += Time.deltaTime;

        if (_heavyChargeTimer >= heavyChargeTime)
            ExecuteHeavyHit();
    }

    private void ExecuteHeavyHit()
    {
        _isChargingHeavy = false;
        _heavyHitExecuted = true;
        StopLoopSound();

        if (!_canHitEnemy || _targetInRange == null)
        {
            PlaySound(missHitsFX);
            return;
        }

        PlaySound(heavyHitsFX);
        StartCoroutine(HitStop(hitStopDuration * 1.5f));
        cameraShake?.Shake(heavyShakeIntensity, shakeDuration * 1.2f);

        DoHeavyHit();
            
    }

    private void CancelHeavyCharge()
    {
        if (_heavyHitExecuted) return;

        _isChargingHeavy = false;
        _heavyChargeTimer = 0f;
        StopLoopSound();
    }

    #endregion

    #region === DAMAGE RECEIVED ===

    public void TakeDamage(float damage, Transform attacker, float knockbackForce)
    {
        currentHealth -= damage;

        PlayHitFX(attacker, knockbackForce);
        StartCoroutine(FlashDamage());
        ApplyKnockback(attacker, knockbackForce);

        Vector3 hitDirection = (transform.position - attacker.position).normalized;
        fakeHitAnimation?.PlayHit(hitDirection);

        ShakeOnHit(knockbackForce);

        if (currentHealth <= 0)
            Die();
    }

    #endregion

    #region === GAMEPLAY RULES ===

    public bool IsDead()
    {
        return _isDead;
    }
    
    private void CheckOutOfBounds()
    {
        if (_isDead) return;

        if (transform.position.y <= deathY)
            Die();
    }

    private void ApplyKnockback(Transform attacker, float force)
    {
        Vector3 dir = (transform.position - attacker.position).normalized;
        Vector3 finalForce = dir * force;
        finalForce.y = knockbackUpForce;

        rb.AddForce(finalForce, ForceMode.Impulse);
    }

    private void Die()
    {
        if (_isDead) return;
        
        _isDead = true;
        
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        OnPlayerDied?.Invoke(this);
    }

    public void NotifyEnemyEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamageable dmg) && other.gameObject != gameObject)
        {
            _canHitEnemy = true;
            _targetInRange = dmg;
        }
    }

    public void NotifyEnemyExit(Collider other)
    {
        if (other.GetComponent<IDamageable>() == _targetInRange)
        {
            _canHitEnemy = false;
            _targetInRange = null;
        }
    }

    #endregion
    
    #region === EVENTS ===
    public static event Action<PlayerController> OnPlayerDied;
    #endregion

    #region === FEEDBACK ===

    private void PlayHitFX(Transform attacker, float force)
    {
        ParticleSystem fx = Mathf.Approximately(force, knockbackHeavyForce)
            ? heavyHitFX
            : basicHitFX;

        if (fx == null) return;

        Vector3 dir = (transform.position - attacker.position).normalized;
        ParticleSystem instance = Instantiate(fx, transform.position + dir * 0.5f, Quaternion.LookRotation(dir));
        
        Destroy(instance.gameObject, instance.main.duration + instance.main.startLifetime.constantMax);
    }

    private void ShakeOnHit(float force)
    {
        if (cameraShake == null) return;

        float intensity = Mathf.Approximately(force, knockbackHeavyForce)
            ? heavyShakeIntensity * 1.2f
            : basicShakeIntensity;

        cameraShake.Shake(intensity, shakeDuration * 0.8f);
    }

    private IEnumerator FlashDamage()
    {
        _meshRenderer.material.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        _meshRenderer.material.color = _originalColor;
    }

    private IEnumerator HitStop(float duration)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }

    #endregion

    #region === AUDIO HELPERS ===

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
            audioSource.PlayOneShot(clip);
    }

    private void PlayLoopSound(AudioClip clip)
    {
        if (clip == null) return;

        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.Play();
    }

    private void StopLoopSound()
    {
        audioSource.loop = false;
        audioSource.Stop();
    }
    #endregion
}

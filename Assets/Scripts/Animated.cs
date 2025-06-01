using UnityEngine;
public class Animated : MonoBehaviour
{
    [SerializeField] private Animator animator;
    
    private IMove _move;
    private IAttack _attack;
    private ITakeDamage _takeDamage;
    
    private static readonly int IsMoving = Animator.StringToHash("isMoving");
    private static readonly int IsAttacking = Animator.StringToHash("isAttacking");
    private static readonly int IsTakingDamage = Animator.StringToHash("isTakingDamage");

    private void Awake()
    {
        _move = GetComponent<IMove>();
        _attack = GetComponent<IAttack>();
        _takeDamage = GetComponent<ITakeDamage>();
    }

    private void OnEnable()
    {
        if (_move != null)
        {
            _move.IsMoving += OnMoveStateChanged;
        }

        if (_attack != null)
        {
            _attack.IsAttacking += OnAttackStateChanged;
        }
        
        if (_takeDamage != null)
        {
            _takeDamage.IsTakingDamage += OnTakeDamageStateChanged;
        }
    }

    private void OnDisable()
    {
        if (_move != null)
        {
            _move.IsMoving -= OnMoveStateChanged;
        }

        if (_attack != null)
        {
            _attack.IsAttacking -= OnAttackStateChanged;
        }
        
        if (_takeDamage != null)
        {
            _takeDamage.IsTakingDamage -= OnTakeDamageStateChanged;
        }
    }

    private void OnMoveStateChanged(bool isMoving)
    {
        animator.SetBool(IsMoving, isMoving);
    }
    
    private void OnAttackStateChanged(bool isAttacking)
    {
        animator.SetBool(IsAttacking, isAttacking);
    }
    
    private void OnTakeDamageStateChanged(bool isTakingDamage)
    {
        animator.SetBool(IsTakingDamage, isTakingDamage);
    }
}

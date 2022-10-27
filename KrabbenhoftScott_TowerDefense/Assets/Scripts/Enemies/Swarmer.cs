using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swarmer : Enemy
{
    [Header("Swarmer Settings")]
    [SerializeField] float _legHeight = 0.075f;
    [SerializeField] float _legSpeed = 25f;
    [SerializeField] float _chompAngle = 75f;
    [SerializeField] float _chompSpeed = 15f;
    [SerializeField] float _chompInterval = 0.5f;

    [Header("Spawn Randomization Settings")]
    [SerializeField] float _chanceToSpawnWithPhysicalResistance = 0.15f;
    [SerializeField] float _chanceToSpawnWithSpecialResistance = 0.15f;

    [Header("Graphic Settings")]
    [SerializeField] Material m_specialDefense;
    [SerializeField] Material m_physicalDefense;
    
    Player _nearestPlayer;
    Rigidbody _rb;
    Transform _upperJaw;
    Transform _legSet1;
    Transform _legSet2;
    float _chompTimer = 0f;
    float _moveModifier = 1f;
    float _targetFindInterval = 1f;
    bool _chomp;
    
    protected override void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _upperJaw = transform.GetChild(0);
        _legSet1 = transform.GetChild(2);
        _legSet2 = transform.GetChild(3);
        _nearestPlayer = FindNearestPlayer();

        // RandomizeStats();

        base.Awake();
    }
    
    void Update()
    {
        AnimateLegs();
        AnimateMouth();
    }

    void RandomizeStats()
    {
        float percent = Random.Range(0f, 1f);
        if (percent <= _chanceToSpawnWithPhysicalResistance)
        {
            _health.PhysicalDefenseModifier = 0.5f;
            SetMaterial(m_physicalDefense);
        }
        else if (percent <= _chanceToSpawnWithPhysicalResistance + _chanceToSpawnWithSpecialResistance)
        {
            _health.SpecialDefenseModifier = 0.5f;
            SetMaterial(m_specialDefense);
        }

        float sizeModifier = Random.Range(0f, 0.75f);
        transform.localScale *= (1 + sizeModifier);
        _health.MaxHealth = (int)(_health.MaxHealth * (1 + sizeModifier));
    }

    void SetMaterial(Material material)
    {
        //
    }
    
    protected override void Move()
    {
        if (Time.time % _targetFindInterval == 0)
        {
            _nearestPlayer = FindNearestPlayer();
        }
            
        if (_nearestPlayer != null)
        {
            Vector3 moveOffset = _nearestPlayer.transform.position - _rb.position;
            _rb.MovePosition(_rb.position + moveOffset * _moveSpeed / Vector3.Distance(transform.position, _nearestPlayer.transform.position) * _moveModifier * Time.deltaTime);

            transform.LookAt(new Vector3(_nearestPlayer.transform.position.x, transform.position.y, _nearestPlayer.transform.position.z));
        }
    }

    public override int GetSpawnCountOfTurn(int turn)
    {
        int unit = (int)Mathf.Round(Mathf.Pow(1.5f, turn));
        int min = Mathf.Clamp(unit - 5, 1, unit);
        int max = unit + 5;

        return (int)Random.Range(min, max);
    }

    Player FindNearestPlayer()
    {
        Player[] _allPlayers = (Player[])FindObjectsOfType(typeof(Player));
        if (_allPlayers.Length == 0)
        {
            return null;
        }
        else if (_allPlayers.Length == 1)
        {
            return _allPlayers[0];
        }
        else
        {
            Player _currentNearestPlayer = _allPlayers[0];

            for (int i = 1; i < _allPlayers.Length; i++)
            {
                if (Vector3.Distance(transform.position, _allPlayers[i].transform.position) < Vector3.Distance(transform.position, _currentNearestPlayer.transform.position))
                {
                    _currentNearestPlayer = _allPlayers[i];
                }
            }

            return _currentNearestPlayer;
        }
    }

    void AnimateLegs()
    {
        _legSet1.localPosition = new Vector3(0, _legHeight * Mathf.Sin(Time.time * _legSpeed), 0);
        _legSet2.localPosition = new Vector3(0, -_legHeight * Mathf.Sin(Time.time * _legSpeed), 0);
    }

    void AnimateMouth()
    {
        _chompTimer += Time.deltaTime;
        
        if (!_chomp && _chompTimer >= (Mathf.PI / _chompSpeed) + _chompInterval && Random.Range(0, 2) >= 1)
        {
            _chomp = true;
            _chompTimer = 0;
        }
        if (_chomp)
        {
            _upperJaw.localRotation = Quaternion.Euler(-_chompAngle * Mathf.Sin(_chompTimer * _chompSpeed), 0, 0);

            if (_chompTimer >= Mathf.PI / _chompSpeed)
            {
                _upperJaw.localRotation = Quaternion.Euler(Vector3.zero);
                _chomp = false;
            }
        }
    }

    void OnDisable()
    {
        base.OnDisable();
        
        Destroy(this.gameObject);
    }
}

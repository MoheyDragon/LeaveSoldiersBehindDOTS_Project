using Unity.Entities;
namespace DotsBattle {
    public partial class PerformSpellSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _ecbSystem;
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<SpellExecutionData>();
        }
        protected override void OnStartRunning()
        {
            _ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = _ecbSystem.CreateCommandBuffer().AsParallelWriter();
            var spellExecution = GetSingleton<SpellExecutionData>();

            var battleEntityQuery = EntityManager.CreateEntityQuery(typeof(HealthPoints), typeof(TeamID));

            switch(spellExecution.TeamId)
            {
                case 0:
                    var team0 = new TeamID { Value = 0 };
                    battleEntityQuery.SetSharedComponentFilter(team0);
                    break;
                case 1:
                    var team1 = new TeamID { Value = 1 };
                    battleEntityQuery.SetSharedComponentFilter(team1);
                    break;
            }
            var newSpell = new PreformSpellJob
            {
                effectAmount = spellExecution.EffectAmount,
                ECB = ecb
            };
            newSpell.ScheduleParallel(battleEntityQuery);
            EntityManager.RemoveComponent<SpellExecutionData>(GetSingletonEntity<SpellExecutionData>());
        }

    }
}
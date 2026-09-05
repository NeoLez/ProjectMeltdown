using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    public class ClimateModifier : MonoBehaviour
    {
        //tener un listya de los que pasan y usarla para notificar a todos lo paquetes que empiezen a perder vida
        //serviria para iniciar y finalizar afectar a cada paquete--> REEMPLAZARLO POR UN DICCIONARIO QUE TENGA LAS CONDICIONES Y EL PAQUETE
        public List<DeliveryPackageItem> packages = new();

        //agregar quetambien agarro SOLO los que tengan la condicion climatica marcada, no cualquiera
        [SerializeField] ClimateConditions packageConditions;


        private void OnTriggerEnter(Collider other)
        {
            if(other.TryGetComponent(out DeliveryPackageItem packageController))
            {
                //chequear si la lista ya contiene ese packate
                if (!packages.Contains(packageController))
                {
                    packages.Add(packageController);
                    TriggerEffects(packages);
                }

            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out DeliveryPackageItem packageController))
            {
                //chequear si la lista ya contiene ese packate
                if (packages.Contains(packageController))
                {
                    DeactivateEffects();
                }
            }
        }


        private void TriggerEffects(List<DeliveryPackageItem> currentPackages)
        {
            for (int i = 0; i < currentPackages.Count; i++)
            {
                currentPackages[i].IsInAffectionZone(true);
            }
        }

        //un vez afuera de la zona eliminarlos
        private void DeactivateEffects()
        {
            if (packages.Count > 0)
            {
                for (int i = 0; i < packages.Count; i++)
                {
                    packages[i].IsInAffectionZone(false);
                }
                packages.Clear();
            }
        }
    }
}

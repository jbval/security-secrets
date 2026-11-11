#Same example than env folder but with a vault to manage secrets
OpenBao (Fork Hashicorp )

=> Connect using token in docker
Create Engine : kv, path : monAppli
Creer un secret : AppSettings
Créer des valeurs : format AppSettings:login => pour surcharger l'appsettings
Créer une policy :
Nom: monapliacl
path "monAppli" {
  capabilities = ["read"]
}

path "monAppli/data/AppSettings" {
  capabilities = ["read"]
}


Ouvrir cli :
bao write auth/token/create policies="monappliacl" ttl="24h"

=> notre compte a maintenant les droits
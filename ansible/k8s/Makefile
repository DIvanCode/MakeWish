ingress:
	ansible-playbook -i inventory playbooks/ingress.yml

postgres:
	ansible-playbook -i inventory playbooks/postgres.yml --ask-vault-pass

neo4j:
	ansible-playbook -i inventory playbooks/neo4j.yml --ask-vault-pass

rabbit:
	ansible-playbook -i inventory playbooks/rabbit.yml --ask-vault-pass

users:
	ansible-playbook -i inventory playbooks/user-service.yml --ask-vault-pass

wishes:
	ansible-playbook -i inventory playbooks/wish-service.yml --ask-vault-pass

users-ports:
	kubectl port-forward -n makewish svc/user-service 5001:5001 --kubeconfig twc-witty-cetus-config.yml

wishes-ports:
	kubectl port-forward -n makewish svc/wish-service 5002:5002 --kubeconfig twc-witty-cetus-config.yml
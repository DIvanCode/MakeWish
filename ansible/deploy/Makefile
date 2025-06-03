users:
	ansible-playbook -i inventory playbooks/user-service.yml --ask-pass --ask-vault-pass --extra-vars="version=$(version)"

wishes:
	ansible-playbook -i inventory playbooks/wish-service.yml --ask-pass --ask-vault-pass --extra-vars="version=$(version)"

neo4j:
	ansible-playbook -i inventory playbooks/neo4j.yml --ask-pass --ask-vault-pass

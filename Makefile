users:
	ansible-playbook -i ansible/inventory ansible/playbooks/build/user-service.yml
	ansible-playbook -i ansible/inventory ansible/playbooks/deploy/user-service.yml --ask-pass --ask-vault-pass

wishes:
	ansible-playbook -i ansible/inventory ansible/playbooks/build/wish-service.yml
	ansible-playbook -i ansible/inventory ansible/playbooks/deploy/wish-service.yml --ask-pass --ask-vault-pass

neo4j:
	ansible-playbook -i ansible/inventory ansible/playbooks/deploy/neo4j.yml --ask-pass --ask-vault-pass

.PHONY: neo4j prometheus node_exporter grafana

neo4j:
	ansible-playbook -i inventory playbooks/neo4j.yml --ask-pass --ask-vault-pass

prometheus:
	ansible-playbook -i inventory playbooks/prometheus.yml -vvvv --ask-pass

node_exporter:
	ansible-playbook -i inventory playbooks/node_exporter.yml --ask-pass

grafana:
	ansible-playbook -i inventory playbooks/grafana.yml --ask-pass
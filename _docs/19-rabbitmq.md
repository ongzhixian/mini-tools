# RabbitMQ

## Environment Variables

ERLANG_HOME=C:\Apps\erlang

RABBITMQ_SERVER=C:\Apps\RabbitMQ\rabbitmq_server-3.9.12



# Install as service

rabbitmq-service.bat install

rabbitmq-plugins enable rabbitmq_management

dotnet add package RabbitMQ.Client

# Start

C:\Apps\RabbitMQ\rabbitmq_server-3.9.12\sbin\rabbitmq-server.bat 

Or run detached (in the background)
C:\Apps\RabbitMQ\rabbitmq_server-3.9.12\sbin\rabbitmq-server.bat -detached
 

rabbitmqctl.bat stop
rabbitmqctl.bat status

# Ports


4369        : epmd, a peer discovery service used by RabbitMQ nodes and CLI tools
5672, 5671  : used by AMQP 0-9-1 and 1.0 clients without and with TLS
25672       : used for inter-node and CLI tools communication (Erlang distribution server port) 
              and is allocated from a dynamic range (limited to a single port by default, computed as AMQP port + 20000). Unless external connections on these ports are really necessary (e.g. the cluster uses federation or CLI tools are used on machines outside the subnet), these ports should not be publicly exposed. See networking guide for details.
35672-35682 : used by CLI tools (Erlang distribution client ports) for communication with nodes 
              and is allocated from a dynamic range (computed as server distribution port + 10000 through server distribution port + 10010). See networking guide for details.
15672       : HTTP API clients, management UI and rabbitmqadmin (only if the management plugin is enabled)
61613, 61614: STOMP clients without and with TLS (only if the STOMP plugin is enabled)
1883, 8883  : MQTT clients without and with TLS, if the MQTT plugin is enabled
15674       : STOMP-over-WebSockets clients (only if the Web STOMP plugin is enabled)
15675       : MQTT-over-WebSockets clients (only if the Web MQTT plugin is enabled)


# User

rabbitmqctl.bat add_user 'username' '9a55f70a841f18b97c3a7db939b7adc9e34a0f1d'
rabbitmqctl.bat list_users
rabbitmqctl.bat list_users --formatter=json
rabbitmqctl.bat delete_user 'username'


# First  ".*" for configure permission on every entity
# Second ".*" for write permission on every entity
# Third  ".*" for read permission on every entity
rabbitmqctl.bat set_permissions -p 'custom-vhost' 'username' '.*' '.*' '.*'
rabbitmqctl.bat clear_permissions -p 'custom-vhost' 'username'


# Reference

https://www.rabbitmq.com/install-windows-manual.html

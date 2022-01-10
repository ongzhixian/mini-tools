# Kubernetes

docker login

docker tag pizzafrontend [YOUR DOCKER USER NAME]/pizzafrontend
docker tag pizzabackend [YOUR DOCKER USER NAME]/pizzabackend

docker push [YOUR DOCKER USER NAME]/pizzafrontend
docker push [YOUR DOCKER USER NAME]/pizzabackend

kubectl get pods

kubectl apply -f backend-deploy.yml
kubectl apply -f frontend-deploy.yml
kubectl apply -f deploy-testapp.yml


kubectl scale --replicas=5 deployment/pizzabackend
kubectl scale --replicas=1 deployment/pizzabackend

kubectl delete pod pizzafrontend-5b6cc765c4-hjpx4


kubectl get services


kubectl delete -f frontend-deploy.yml
kubectl delete -f backend-deploy.yml


Run `backend-deploy.yml`. 
It will download the image from Docker Hub and create the container.
`kubectl apply` command is asynchronous; 
The kubectl apply command will return quickly. But the container creation may take a while. To view the progress, use `kubectl get pods`



// Still not sure the how to properly use the following commands:

kubectl create deployment guestbook --image=testapp
kubectl expose deployment guestbook --type="NodePort" --port=3000
kubectl get service guestbook
kubectl get nodes -o wide

kubectl delete deployment guestbook


## Examples


```yml:deploy-pod-testapp.yml
---
apiVersion: v1
kind: Pod
metadata:
  name: testapp1
spec:
  containers:
  - name: testapp2
    image: testapp
    imagePullPolicy: Never
    ports:
    - containerPort: 80
```
Deploys an image to a pod.

Run a local image by setting the imagePullPolicy to Never.
Creates 2 containers:
1.       k8s_POD_testapp1_default_fee1d0cc-b70c-48c2-afbc-7e9aea7c28ca_0
2.  k8s_testapp2_testapp1_default_fee1d0cc-b70c-48c2-afbc-7e9aea7c28ca_0
    k8s_<container-name>_<pod_metaName>

`kubectl apply -f deploy-pod-testapp.yml`
`kubectl delete -f deploy-pod-testapp.yml`




```yml:deploy-deployment-testapp.yml
---
apiVersion: apps/v1
kind: Deployment
metadata:
  name: testapp1
spec:
  replicas: 1
  template:
    metadata:
      labels:
        app: testapp2
    spec:
      containers:
      - name: testapp3
        image: testapp:latest
        imagePullPolicy: Never
        ports:
        - containerPort: 80
  selector:
    matchLabels:
      app: testapp2
```

Creates 2 containers:
     k8s_POD_testapp1-55bdb65b9c-45ppz_default_55b81b49-297f-4c0b-ab98-23c88a78a503_0
k8s_testapp3_testapp1-55bdb65b9c-45ppz_default_55b81b49-297f-4c0b-ab98-23c88a78a503_0
k8s_<container-name>_<deployment-name>

kubectl apply -f deploy-deployment-testapp.yml
kubectl delete -f deploy-deployment-testapp.yml

kubectl get deployments
kubectl scale --replicas=2 deployments/testapp1


Examples from tutorials


```Example for backend
---
apiVersion: apps/v1
kind: Deployment
metadata:
  name: pizzabackend
spec:
  replicas: 1
  template:
    metadata:
      labels:
        app: pizzabackend
    spec:
      containers:
      - name: pizzabackend
        image: [YOUR DOCKER USER NAME]/pizzabackend:latest
        ports:
        - containerPort: 80
        env:
        - name: ASPNETCORE_URLS
          value: http://*:80
  selector:
    matchLabels:
      app: pizzabackend
---
apiVersion: v1
kind: Service
metadata:
  name: pizzabackend
spec:
  type: ClusterIP
  ports:
  - port: 80
  selector:
    app: pizzabackend
```

The first portion defines a deployment `spec` for the container that will be deployed into Kubernetes. 
It specifies there will be one replica, where to find the container image from, which ports to open on the container, and sets some environment variables. 
This first portion also defines labels and names that the container and spec can be referenced by.

The second portion defines that the container will run as a Kubernetes ClusterIP. 
ClusterIPs does not expose an external IP address. 
It is only accessible from other services running from within the same Kubernetes cluster.


```Example for web frontend
---
apiVersion: apps/v1
kind: Deployment
metadata:
  name: pizzafrontend
spec:
  replicas: 1
  template:
    metadata:
      labels:
        app: pizzafrontend
    spec:
      containers:
      - name: pizzafrontend
        image: [YOUR DOCKER USER NAME]/pizzafrontend
        ports:
        - containerPort: 80
        env:
        - name: ASPNETCORE_URLS
          value: http://*:80
        - name: backendUrl
          value: http://pizzabackend
  selector:
    matchLabels:
      app: pizzafrontend
---
apiVersion: v1
kind: Service
metadata:
  name: pizzafrontend
spec:
  type: LoadBalancer
  ports:
  - port: 80
  selector:
    app: pizzafrontend
```


# Secrets

```yml
apiVersion: v1
kind: Secret
metadata:
  name: test-secret
data:
  username: bXktYXBw
  password: Mzk1MjgkdmRnN0pi
```

Note: Values for data field need to be encoded in base64.


kubectl apply -f https://k8s.io/examples/pods/inject/secret.yaml

kubectl get secret test-secret

kubectl describe secret test-secret

Create secret directly
kubectl create secret generic test-secret --from-literal='username=my-app' --from-literal='password=39528$vdg7Jb'

```yml
apiVersion: v1
kind: Pod
metadata:
  name: secret-test-pod
spec:
  containers:
    - name: test-container
      image: nginx
      volumeMounts:
        # name must match the volume name below
        - name: secret-volume
          mountPath: /etc/secret-volume
  # The secret data is exposed to Containers in the Pod through a Volume.
  volumes:
    - name: secret-volume
      secret:
        secretName: test-secret
```

This maps each secret as individual file under the mountPath:
Meaning `ls /etc/secret-volume` will list:
1.  password    (/etc/secret-volume/password)
2.  username    (/etc/secret-volume/username)



Environment variables using Secret data 
                              <secret-name>               <secret-key>
kubectl create secret generic backend-user --from-literal=backend-username='backend-admin'
kubectl create secret generic db-user      --from-literal=db-username='db-admin'

```yml:read secret into environment variable
apiVersion: v1
kind: Pod
metadata:
  name: envvars-multiple-secrets
spec:
  containers:
  - name: envars-test-container
    image: nginx
    env:
    - name: BACKEND_USERNAME
      valueFrom:
        secretKeyRef:
          name: backend-user
          key: backend-username
    - name: DB_USERNAME
      valueFrom:
        secretKeyRef:
          name: db-user
          key: db-username
          
```

Assign the backend-username value defined in the Secret to the SECRET_USERNAME environment variable in the Pod specification.



Use `envFrom` to define all of the Secret's data as container environment variables. 
The key from the Secret becomes the environment variable name in the Pod.

```yml
apiVersion: v1
kind: Pod
metadata:
  name: envfrom-secret
spec:
  containers:
  - name: envars-test-container
    image: nginx
    envFrom:
    - secretRef:
        name: test-secret
```


## Environment variables


```yml
apiVersion: v1
kind: Pod
metadata:
  name: dapi-envars-fieldref
spec:
  containers:
    - name: test-container
      image: k8s.gcr.io/busybox
      command: [ "sh", "-c"]
      args:
      - while true; do
          echo -en '\n';
          printenv MY_NODE_NAME MY_POD_NAME MY_POD_NAMESPACE;
          printenv MY_POD_IP MY_POD_SERVICE_ACCOUNT;
          sleep 10;
        done;
      env:
        #Pod fields as values for environment variables
        - name: MY_NODE_NAME
          valueFrom:
            fieldRef:
              fieldPath: spec.nodeName
        - name: MY_POD_NAME
          valueFrom:
            fieldRef:
              fieldPath: metadata.name
        - name: MY_POD_NAMESPACE
          valueFrom:
            fieldRef:
              fieldPath: metadata.namespace
        - name: MY_POD_IP
          valueFrom:
            fieldRef:
              fieldPath: status.podIP
        - name: MY_POD_SERVICE_ACCOUNT
          valueFrom:
            fieldRef:
              fieldPath: spec.serviceAccountName
        # Container fields as values for environment variables
        - name: MY_CPU_REQUEST
          valueFrom:
            resourceFieldRef:
              containerName: test-container
              resource: requests.cpu
        - name: MY_CPU_LIMIT
          valueFrom:
            resourceFieldRef:
              containerName: test-container
              resource: limits.cpu
        - name: MY_MEM_REQUEST
          valueFrom:
            resourceFieldRef:
              containerName: test-container
              resource: requests.memory
        - name: MY_MEM_LIMIT
          valueFrom:
            resourceFieldRef:
              containerName: test-container
              resource: limits.memory
  restartPolicy: Never
```


```yml : expose Pod and Container fields to a running Container
apiVersion: v1
kind: Pod
metadata:
  name: kubernetes-downwardapi-volume-example
  labels:
    zone: us-est-coast
    cluster: test-cluster1
    rack: rack-22
  annotations:
    build: two
    builder: john-doe
spec:
  containers:
    - name: client-container
      image: k8s.gcr.io/busybox
      command: ["sh", "-c"]
      args:
      - while true; do
          if [[ -e /etc/podinfo/labels ]]; then
            echo -en '\n\n'; cat /etc/podinfo/labels; fi;
          if [[ -e /etc/podinfo/annotations ]]; then
            echo -en '\n\n'; cat /etc/podinfo/annotations; fi;
          sleep 5;
        done;
      volumeMounts:
        - name: podinfo
          mountPath: /etc/podinfo
  volumes:
    - name: podinfo
      downwardAPI:
        items:
          # Store Pod fields
          - path: "labels"
            fieldRef:
              fieldPath: metadata.labels
          - path: "annotations"
            fieldRef:
              fieldPath: metadata.annotations
          # Store Container fields
          - path: "cpu_limit"
            resourceFieldRef:
              containerName: client-container
              resource: limits.cpu
              divisor: 1m
          - path: "cpu_request"
            resourceFieldRef:
              containerName: client-container
              resource: requests.cpu
              divisor: 1m
          - path: "mem_limit"
            resourceFieldRef:
              containerName: client-container
              resource: limits.memory
              divisor: 1Mi
          - path: "mem_request"
            resourceFieldRef:
              containerName: client-container
              resource: requests.memory
              divisor: 1Mi
```

cat /etc/podinfo/labels
cat /etc/podinfo/annotations


# Setup

 kubectl config get-contexts

 kubectl config use-context docker-desktop

  kubectl get nodes

# Reference

https://docs.docker.com/desktop/kubernetes/


https://docs.microsoft.com/en-us/learn/modules/dotnet-deploy-microservices-kubernetes/

https://jamesdefabia.github.io/docs/user-guide/kubectl-cheatsheet/

https://kubernetes.io/docs/tasks/inject-data-application/environment-variable-expose-pod-information/

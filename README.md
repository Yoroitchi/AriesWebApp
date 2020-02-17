# AriesWebApp
This repository contain the code simulating three *Self-Sovereign Identity* Agents. Every agent has been developped using Aries .NET framework (https://github.com/hyperledger/aries-framework-dotnet).
This project was made as a proof of concept, each agent has a specific role following standards of SSI as described by the Verifiable Claims Working Group of the W3C (https://w3c.github.io/vc-data-model) 

## What you will need
The following instructions work for Ubuntu-18.04 distribution

Clone this repo
```
$git clone https://github.com/Yoroitchi/AriesWebApp.git
```
Move into the cloned file, you should have the following arborescence
```
./AriesWebApp
|____________AriesWebApp
|____________AriesWebApp.sln
```
1. Be sure to have `docker` and `docker-compose` installed
2. Clone indy-sdk (https://github.com/hyperledger/indy-sdk) on your machine 
```
$git clone https://github.com/hyperledger/indy-sdk
```
3. Add `192.168.56.101` to one of your network interface
```
$sudo ip addr add 192.168.56.101/24 dev [your interface]
```
4. build a test pool compatible with the agents, as indicated in indy-sdk repository. A pool genesis transaction file is already in the agents, you need to build the pool on the ip 192.168.56.101.
First go into the cloned file, then build and run the pool
```
$cd indy-sdk
$docker build --build-arg pool_ip=192.168.56.101 -f ci/indy-pool.dockerfile -t indy_pool .
$docker run -itd -p 192.168.56.101:9701-9708:9701-9708 indy_pool
```
5. Install indy-cli as indicated in indy-sdk repo
```
$sudo apt-key adv --keyserver keyserver.ubuntu.com --recv-keys CE7709D068DB5E88
$sudo add-apt-repository "deb https://repo.sovrin.org/sdk/deb (xenial|bionic) {release channel}"
$sudo apt-get update
$sudo apt-get install -y {library}
```
* Replace (xenial | bionic) with bionic for Ubuntu 18.04
* {release channel} must be replaced with `master`, `rc` or `stable`
  * choose `stable`
* {library} must be replaced with libindy, libnullpay, libvcx or indy-cli
  * choose indy-cli
  * More details on other libraries in indy-sdk repo
6. Using indy-cli, create a pool named `Aries` with the pool genesis transaction
  * You can find it in the running pool container at `/var/lib/indy/sandbox/pool_genesis_transaction`
  * Or you can use the one in the project `./AriesWebApp/AriesTest.txn`
  ```
    #Run indy-cli
    $indy-cli
    #Create the pool
    indy>pool create Aries gen_txn_file=pool_genesis_transaction
  ```
 7. From now you will build a docker container for each agent. One branch is dedicated to each one
  ```
    #Checkout to branch 
    $git checkout Holder5000
    #Build the docker container
    $docker build -t holder -f ./AriesWebApp/docker/dockerfile .
  ```
   * Repeat with branches Issuer7000 and Verifier8000
   * Replace `holder` with `issuer` and `verifier` according to your current branch
  8. Run every container on the host docker network
  ```
  docker run -itd -p {port}:{port} --network=host {actor}
  ```
   * {port} must match with the branch, i.e. for Verifier8000, {port}=8000
   * {actor} is the container's tag
  9. Open a browser with 3 tabs, in each tab go to `http://[::1]:{port}`
  10. Play with agents


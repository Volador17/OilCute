clear;
load CX.mat;
load CY.mat;

cx = sgdiff(CX,21,2);
cx = smooth(cx,5);
cx = [cx(132:313,:);cx(469:676,:)];
%cx = normpathlength(cx);

cy = CY(:,1);

[net,Loads,Scores,Weights,b,yreg,ye,R,sec,centerSpecData,centerCompValue,Score_length,mdt,nndt] = PLSANNTrain(cx,cy,10,'trainlm',5,'logsig','purelin',100,1.000000000000000e-04,5);

 


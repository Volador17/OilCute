load CX.mat;
load CY.mat;

cx = sgdiff(CX,21,2);
cx = smooth(cx,5);
cx = [cx(132:313,:);cx(469:676,:)];

cy = CY(:,1);
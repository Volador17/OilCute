
load VX.mat;
load VY.mat;

vx = sgdiff(VX,21,2);
vx = smooth(vx,5);
vx = [vx(132:313,:);vx(469:676,:)];

vy = VY(:,1);
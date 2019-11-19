%标准正态变量变换（Standard Normal Variate Transformation，SNV）。
%X：吸光度矩阵；
%SX：处理后的吸光度矩阵
function SX=snv(X)
	[rx,cx]=size(X);
	stdx=std(X,0,1);
	mx=mean(X,1);
	SX=(X-mx(ones(rx,1),:))./stdx(ones(rx,1),:);
end
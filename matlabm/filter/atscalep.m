% 标准化程序，用于校正集外样品，如果用于校正集，参见atscale；
% input
% x:吸光度矩阵；
% mx:平均光谱；
% sx:标准偏差光谱；
% output
% xc:处理后的吸光度矩阵；

function [xc]=atscalep(x,mx,sx);
	[m,n]=size(x);
	II=find(sx==0);%寻找sx为0的项,在计算xc时不予计算，而设为0
	xc=zeros(m,n);
	III=1:m;
	III(II)=[];
	xc(III,:)=(x(III,:)-mx(III,:)*ones(1,n))./(sx(III,:)*ones(1,n));
end